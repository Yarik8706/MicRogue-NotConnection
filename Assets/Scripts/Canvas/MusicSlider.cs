using UnityEngine.Audio;

namespace Canvas
{
    public class MusicSlider : PixelSliderControl
    {
        public AudioMixerGroup musicGroup;
        public string musicValueName;

        protected override void StartCount()
        {
            var safeValue = PlayerPrefsSafe.GetInt(musicValueName, countNow);
            SetCount(safeValue);
        }
        
        protected override void AddEvent()
        {
            base.AddEvent();
            musicGroup.audioMixer.SetFloat(musicValueName, value);
            PlayerPrefsSafe.SetInt(musicValueName, countNow);
        }
    
        protected override void DeleteEvent()
        {
            base.DeleteEvent();
            musicGroup.audioMixer.SetFloat(musicValueName, value);
            PlayerPrefsSafe.SetInt(musicValueName, countNow);
        }

        protected override void SetValue(int value)
        {
            base.SetValue(value);
            if (value == minValue)
            {
                musicGroup.audioMixer.SetFloat(musicValueName, -80);
                PlayerPrefsSafe.SetInt(musicValueName, countNow);
            }
            else
            {
                musicGroup.audioMixer.SetFloat(musicValueName, value);
                PlayerPrefsSafe.SetInt(musicValueName, countNow);
            }
        }
    }
}
