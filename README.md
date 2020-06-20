# DerOptimist

Der Optimist is - as is often the case with my tools - the result of a long line of in-house quick-fire tools that were originally meant to just fix a small problem, and fast.

The particular problem here: encoding videos and image sequences into h264 (and other formats) and never really being sure if they quality settings are good enough or the file will fit into your size requirements until after the entire thing is encoded. And then try again. This gets old fast, in my experience. Plus, handing artists mile-long ffmpeg command lines has turned out to be rather less of a success than one hoped. Thus: we needed a GUI that was easy to use and would give us feedback on the quality without wasting everyone's time and patience.

Der Optimist will read anything that ffmpeg is able to handle. For this I'm using a heavily extended and modified fork of https://github.com/cmxl/FFmpeg.NET

To open the source file just drag the video or audio file, or any frame from your image sequence (der Optimist will try its best to determine the sequence) into the main interface (the left one, if you have the dual view open).
Or use the Open dialogs. There are two, because the standard Open dialog is all well and good for single media files, but it's a pain for image sequences (After Effects, I'm looking at you!). So I wrote my own dialog for sequences specifically.
![DerOptimist_BasicInterface](https://user-images.githubusercontent.com/6930367/85209948-6446e980-b33c-11ea-88c3-eddea2414414.png)

Once opened, select a preset or build your own. To keep things easy for the end use, the encoders are hard coded currently with specific parameter ranges. To add you own you'll have to add them in source. Sorry. Please remember to send a pull request...
Now, with everything set up just so, press Render(Preview). Der Optimist will now render a small sequence starting at the current playhead position and show you the encoded result. Like the result? Press Render(Final) and the entire thing is encoded.
![DerOptimist_RenderPreview](https://user-images.githubusercontent.com/6930367/85209950-65781680-b33c-11ea-8e2b-4e5212efa6a5.png)

You can adjust the preview range length with the Manual Range Mode. Combined with the "Render preview range" this will effectively let you do final renders for only a cropped part of the input.

There are a bunch of other filters available based on ffmpeg, they should behave like in the ffmpeg documentation. You can also add manual command line parameters in the appropriate fields.

Under Audio you have the option to replace the audio part entirely with an audio clip of your own (for ADR for example)

There's also the Render Queue and History. The queue is just that: add multiple files either from the main GUI or by dragging them in. Dragged files pick up the last settings defined.
The Render History is for you trial and errors when working out the perfect encoding settings. You can pick the favourite setting from the last few attempts and use that for the actual render.

![DerOptimist_QueueHistory](https://user-images.githubusercontent.com/6930367/85209949-64df8000-b33c-11ea-8e34-323fa12751c3.png)

MediaInfo is also included to give you more information on each file (press F2).

Navigation with the mouse: left drag to skip through the file, middle to pan, wheel to zoom. By default left and right view keep in sync.
