using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Collections;

namespace DerOptimist
{
    public class FileList : IEnumerable<FileMeta>
    {
        public List<FileMeta> meta { get; set; }
        List<string> ignore;
        public string CurrentDirectory { get; set; }
        public bool GroupSequences { get; set; } = true;
        public bool SplitSequenceIfMissingFrames { get; set; } = true;

        public FileList()
        {
            meta = new List<FileMeta>();
        }

        public FileList(string directory, MediaType filter=MediaType.All)
        {
            CurrentDirectory = directory;
            Refresh(filter);
        }

        IEnumerator<FileMeta> IEnumerable<FileMeta>.GetEnumerator()
        {
            return ((IEnumerable<FileMeta>)meta).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<FileMeta>)meta).GetEnumerator();
        }

        public FileMeta this[int element]
        {
            get
            {

                if (element < meta.Count)
                {
                    return meta[element];
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }

            set
            {
                if (element < meta.Count)
                {
                    meta[element] = value;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public FileMeta Add(string file)
        {
            FileMeta fm = FileMeta.Create(file, ref ignore, groupSequences: GroupSequences);
            if (fm != null)
            {
                //meta.Add(fm);
                //Debug.WriteLine(fm.DisplayName);
                //return fm;
                return Add(fm);
            }
            return null;
        }

        public FileMeta Add(FileMeta fm)
        {
            if (fm != null)
            {
                meta.Add(fm);
                Debug.WriteLine(fm.DisplayName);
                return fm;
            }
            return null;
        }

        public void Insert(int index, FileMeta fm)
        {
            meta.Insert(index, fm);
        }

        public void Refresh(MediaType filter)
        {
            if (!Directory.Exists(CurrentDirectory)) return;

            meta = new List<FileMeta>();
            ignore = new List<string>();

            Debug.WriteLine("EnumerateDirectories");
            foreach (var file in Directory.EnumerateDirectories(CurrentDirectory))
            {
                FileMeta fm = FileMeta.Create(file, ref ignore, groupSequences: false);
                if (fm != null)
                {
                    meta.Add(fm);
                    Debug.WriteLine(fm.DisplayName);
                }
            }

            Debug.WriteLine("EnumerateFiles");
            foreach (var file in Directory.EnumerateFiles(CurrentDirectory))
            {
                if (filter != MediaType.All)
                {
                    MediaType mt = Helpers.MediaTypeFromFileName(file);
                    if (mt != filter)
                    {
                        continue;
                    }
                }
                FileMeta fm = FileMeta.Create(file, ref ignore, groupSequences:GroupSequences);
                if (fm != null)
                {
                    meta.Add(fm);
                    Debug.WriteLine(fm.DisplayName);
                }
            }
            Debug.WriteLine("SplitSequenceIfMissingFrames");

            if (SplitSequenceIfMissingFrames)
            {
                List<FileMeta> metaNew = new List<FileMeta>();
                foreach (var fm in meta)
                {
                    if (fm.MediaType == MediaType.ImageSequence)
                    {
                        int start = fm.First;
                        int end = fm.Last;
                        bool original = true;
                        bool sequenceOpen = true;
                        FileMeta tmpMeta = fm.GetCopy();
                        for (int i = start; i <= end; i++)
                        {
                            if (!File.Exists(fm.GetPathWithCounter(i)))
                            {
                                if (sequenceOpen == true)
                                {
                                    if (original == true)
                                    {
                                        original = false;
                                        fm.Last = Math.Max(start, i - 1);
                                        sequenceOpen = false;
                                    }
                                    else
                                    {
                                        tmpMeta.Last = Math.Max(tmpMeta.First, i - 1);
                                        sequenceOpen = false;
                                        metaNew.Add(tmpMeta);
                                    }
                                }
                                else
                                {
                                    // nothing
                                }
                            }
                            else  // file exists
                            {
                                if (sequenceOpen == true)
                                {
                                    // file within sequence, so nothing to do
                                    if (i == end && original == false)
                                    {
                                        tmpMeta.Last = Math.Max(tmpMeta.First, i - 1);
                                        metaNew.Add(tmpMeta);
                                    }
                                }
                                else
                                {
                                    sequenceOpen = true;
                                    tmpMeta = fm.GetCopy();
                                    tmpMeta.First = i;
                                }
                            }
                        }
                    }
                }
                // I love LINQ :)
                Debug.WriteLine("Sort");
                meta = meta.Concat(metaNew).OrderBy(i => i.PatternString).ThenBy(i => i.First).ToList<FileMeta>();
                foreach (var fm in meta)
                {
                    Debug.WriteLine($"{fm.DisplayName}");
                }
            }
        }


    }
}
