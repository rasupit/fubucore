using System;
using System.Linq;

namespace FubuCore.Dates
{
    public class LocalTime : IComparable<LocalTime>
    {
        public static LocalTime AtMachineTime(DateTime time)
        {
            return new LocalTime(time.ToUniversalTime(TimeZoneInfo.Local), TimeZoneInfo.Local);
        }

        public static LocalTime AtMachineTime(TimeSpan time)
        {
            return LocalTime.AtMachineTime(DateTime.Today.Add(time));
        }

        public static LocalTime AtMachineTime(string timeString)
        {
            return LocalTime.AtMachineTime(DateTime.Today.Add(timeString.ToTime()));
        }

        public static LocalTime GuessDayFromTimeOfDay(LocalTime currentTime, TimeSpan timeOfDay)
        {
            var today = currentTime.AtTime(timeOfDay);
            
            
            
            if (Math.Abs(currentTime.Subtract(today).TotalHours) < 12)
            {
                return today;
            }

            var yesterday = today.Add(-1.Days());
            if (Math.Abs(currentTime.Subtract(yesterday).TotalHours) < 12)
            {
                return yesterday;
            }

            return today.Add(1.Days());
        }

        public static LocalTime Now()
        {
            return new LocalTime(DateTime.UtcNow, TimeZoneInfo.Local);
        }

        public LocalTime AtTime(TimeSpan time)
        {
            return BeginningOfDay().Add(time);
        }

        public LocalTime BeginningOfDay()
        {
            var beginningTime = Time.Date.ToUniversalTime(TimeZone);
            return new LocalTime(beginningTime, TimeZone);
        }

        public LocalTime(DateTime utcTime, TimeZoneInfo timeZone)
        {
            TimeZone = timeZone;
            UtcTime = utcTime;
        
        }

        public string Hydrate()
        {
            return "{0}@{1}".ToFormat(UtcTime.ToString("r"), TimeZone.Id);
        }

        public LocalTime(string representation)
        {
            var parts = representation.Split('@');

            if (parts.Count() == 1)
            {
                TimeZone = TimeZoneInfo.Local;
                UtcTime = DateTime.Today.Add(representation.ToTime()).ToUniversalTime();
            }
            else
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(parts[1]);
                UtcTime = DateTime.ParseExact(parts[0], "r", null);
            }


        }

        public TimeZoneInfo TimeZone
        {
            get; private set;
        }

        public DateTime UtcTime
        {
            get; private set;
        }

        public Date Date
        {
            get
            {
                return Time.ToDate();
            }
        }

        public TimeSpan TimeOfDay
        {
            get
            {
                return Time.TimeOfDay;
            }
        }

        public TimeSpan Subtract(LocalTime otherTime)
        {
            return UtcTime.Subtract(otherTime.UtcTime);
        }

        public LocalTime Add(TimeSpan duration)
        {
            return new LocalTime(UtcTime.Add(duration), TimeZone);
        }

        public DateTime Time
        {
            get { return UtcTime.ToLocalTime(TimeZone); }
        }

        public bool Equals(LocalTime other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.TimeZone.Id, TimeZone.Id) && other.UtcTime.Equals(UtcTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (LocalTime)) return false;
            return Equals((LocalTime) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TimeZone != null ? TimeZone.GetHashCode() : 0)*397) ^ UtcTime.GetHashCode();
            }
        }

        public int CompareTo(LocalTime other)
        {
            return UtcTime.CompareTo(other.UtcTime);
        }

        public override string ToString()
        {
            return string.Format("TimeZone: {0}, LocalTime: {1}", TimeZone, Time);
        }

        public static bool operator >(LocalTime left, LocalTime right)
        {
            return left.UtcTime > right.UtcTime;
        }

        public static bool operator <(LocalTime left, LocalTime right)
        {
            return left.UtcTime < right.UtcTime;
        }

        public static bool operator >=(LocalTime left, LocalTime right)
        {
            return left.UtcTime >= right.UtcTime;
        }

        public static bool operator <=(LocalTime left, LocalTime right)
        {
            return left.UtcTime <= right.UtcTime;
        }


    }
}