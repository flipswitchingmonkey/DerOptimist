﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DerOptimist.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.2.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ffmpegBinaryPath {
            get {
                return ((string)(this["ffmpegBinaryPath"]));
            }
            set {
                this["ffmpegBinaryPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string defaultEncoderVideo {
            get {
                return ((string)(this["defaultEncoderVideo"]));
            }
            set {
                this["defaultEncoderVideo"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string defaultOutputPath {
            get {
                return ((string)(this["defaultOutputPath"]));
            }
            set {
                this["defaultOutputPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string defaultPreviewPath {
            get {
                return ((string)(this["defaultPreviewPath"]));
            }
            set {
                this["defaultPreviewPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public double defaultPreviewDuration {
            get {
                return ((double)(this["defaultPreviewDuration"]));
            }
            set {
                this["defaultPreviewDuration"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("25")]
        public string defaultFrameRate {
            get {
                return ((string)(this["defaultFrameRate"]));
            }
            set {
                this["defaultFrameRate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool KeepPreviewFiles {
            get {
                return ((bool)(this["KeepPreviewFiles"]));
            }
            set {
                this["KeepPreviewFiles"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool KeepPreviewFilesHistoryDelete {
            get {
                return ((bool)(this["KeepPreviewFilesHistoryDelete"]));
            }
            set {
                this["KeepPreviewFilesHistoryDelete"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Bookmarks {
            get {
                return ((string)(this["Bookmarks"]));
            }
            set {
                this["Bookmarks"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public double defaultSingleImageDuration {
            get {
                return ((double)(this["defaultSingleImageDuration"]));
            }
            set {
                this["defaultSingleImageDuration"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("c:\\")]
        public string lastOpenedDirectory {
            get {
                return ((string)(this["lastOpenedDirectory"]));
            }
            set {
                this["lastOpenedDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("avi, mp4, mpg, mov, mxf, mkv, ogm, flv, m4v, rm, wmv, webm")]
        public string extensionsVideo {
            get {
                return ((string)(this["extensionsVideo"]));
            }
            set {
                this["extensionsVideo"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("mp3, aiff, aif, m4a, mpa, wma, aac, wav, ogg")]
        public string extensionsAudio {
            get {
                return ((string)(this["extensionsAudio"]));
            }
            set {
                this["extensionsAudio"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("jpg, png, tif, tiff, dpx, exr, dds, tga, iff, bmp, gif, img, jpe, jpeg, pbm, pcx," +
            " pct, ppm, ras, psd, psb, yuv")]
        public string extensionsImage {
            get {
                return ((string)(this["extensionsImage"]));
            }
            set {
                this["extensionsImage"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("preset, com, exe, txt, doc, xls, xlss, js, cpp, DS_Store, rar, zip, exe, com,  js" +
            "on, dat, bin")]
        public string extensionsSkip {
            get {
                return ((string)(this["extensionsSkip"]));
            }
            set {
                this["extensionsSkip"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string defaultEncoderAudio {
            get {
                return ((string)(this["defaultEncoderAudio"]));
            }
            set {
                this["defaultEncoderAudio"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        public double TimeCodeFontSize {
            get {
                return ((double)(this["TimeCodeFontSize"]));
            }
            set {
                this["TimeCodeFontSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("White")]
        public string TimeCodeFontColor {
            get {
                return ((string)(this["TimeCodeFontColor"]));
            }
            set {
                this["TimeCodeFontColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string PresetFolderPath {
            get {
                return ((string)(this["PresetFolderPath"]));
            }
            set {
                this["PresetFolderPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection RecentFiles {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["RecentFiles"]));
            }
            set {
                this["RecentFiles"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int Version {
            get {
                return ((int)(this["Version"]));
            }
            set {
                this["Version"] = value;
            }
        }
    }
}
