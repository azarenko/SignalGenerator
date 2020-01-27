using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SignalGenerator
{
    public class MediaPlayer
    {
        System.Media.SoundPlayer soundPlayer;
        MemoryStream memoryStream;

        public MediaPlayer(byte[] buffer)
        {
            memoryStream = new MemoryStream(buffer, true);
            soundPlayer = new System.Media.SoundPlayer(memoryStream);
        }

        public void Play(byte[] buffer)
        {
            soundPlayer.Stream = new MemoryStream(buffer, true);
            soundPlayer.PlayLooping();
        }

        public void Stop()
        {
            soundPlayer.Stop();
        }
    }
}
