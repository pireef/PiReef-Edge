using System;

namespace UWP_App.Models
{
    public class Measurement
    {
        private string _title;
        private float _value;
        private DateTime _lastRead;
        private string _iconimg;

        public Measurement(string iconimg, DateTime lastRead, float value, string title)
        {
            Iconimg = iconimg;
            LastRead = lastRead;
            Value = value;
            Title = title;
        }

        public string Iconimg { get => _iconimg; set => _iconimg = value; }
        public DateTime LastRead { get => _lastRead; set => _lastRead = value; }
        public float Value { get => _value; set => _value = value; }
        public string Title { get => _title; set => _title = value; }
    }
}
