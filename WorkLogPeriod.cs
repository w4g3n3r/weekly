namespace Weekly
{
    public class WorkLogPeriod
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int Length => EndDate.Subtract(StartDate).Days + 1;
        public Dictionary<char, DateTime> Days { get; set; }
        public bool IsCurrent => StartDate <= DateTime.Today && DateTime.Today <= EndDate;

        public WorkLogPeriod(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;

            Days = new Dictionary<char, DateTime>();

            for (var day = startDate; day <= endDate; day = day.AddDays(1))
            {
                Days.Add("umtwrfs"[(int)day.DayOfWeek], day);
            }
        }

        public WorkLogPeriod(DateTime startDate, int days)
            : this(startDate, startDate.AddDays(days - 1))
        {
        }

        public bool ContainsDate(DateTime date)
        {
            return StartDate <= date && date <= EndDate;
        }
        public DateTime? GetDate(DayOfWeek dayOfWeek)
        {
            return GetDate(WorkLogPeriod.GetDayOfWeek(dayOfWeek));
        }
        public DateTime? GetDate(char dayOfWeek)

        {
            if (Days.ContainsKey(dayOfWeek))
            {
                return Days[dayOfWeek];
            }
            else
            {
                return null;
            }
        }

        override public string ToString()
        {
            return $"{StartDate:yyyy-MM-dd} {EndDate:yyyy-MM-dd}";
        }

        public string ToLongString()
        {
            return $"{StartDate:yyyy-MM-dd (ddd)} - {EndDate:yyyy-MM-dd (ddd)}";
        }

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is WorkLogPeriod other)
            {
                return StartDate == other.StartDate && EndDate == other.EndDate;
            }
            else
            {
                return false;
            }
        }

        public static WorkLogPeriod GetCurrentPeriod(char firstDayOfWeek, int length)
        {
            var today = DateTime.Today;
            var startingDay = GetDayOfWeek(firstDayOfWeek);

            int startOffset = (int)today.DayOfWeek - (int)startingDay;

            if (startOffset < 0)
            {
                startOffset += 7;
            }

            if (startOffset >= length)
            {
                // We're outside of the current work log period
                // Maybe on a weekend or something. Create a new one-off period
                // with just one day... today.
                return new WorkLogPeriod(today, today);
            }
            else
            {
                var startDate = today.AddDays(-startOffset);
                return new WorkLogPeriod(startDate, length);
            }
        }

        public static char GetDayOfWeek(DayOfWeek dayOfWeek)
        {
            return "umtwrfs"[((int)dayOfWeek)];
        }

        public static DayOfWeek GetDayOfWeek(char dayOfWeek)
        {
            return dayOfWeek switch
            {
                'u' => DayOfWeek.Sunday,
                'm' => DayOfWeek.Monday,
                't' => DayOfWeek.Tuesday,
                'w' => DayOfWeek.Wednesday,
                'r' => DayOfWeek.Thursday,
                'f' => DayOfWeek.Friday,
                's' => DayOfWeek.Saturday,
                _ => throw new ArgumentException("Invalid day of week.", nameof(dayOfWeek))
            };
        }
    }
}
