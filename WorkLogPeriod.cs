namespace Weekly
{
    public class WorkLogPeriod
    {
        private const string VALID_WEEKDAYS = "umtwrfs";

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
                Days.Add(VALID_WEEKDAYS[(int)day.DayOfWeek], day);
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
            return GetPeriodForDate(DateTime.Today, firstDayOfWeek, length);
        }

        public static WorkLogPeriod GetLastPeriod(char firstDayOfWeek, int length)
        {
            var currentPeriod = GetCurrentPeriod(firstDayOfWeek, length);

            var lastPeriodStart = currentPeriod.StartDate.Subtract(TimeSpan.FromDays(7));

            return GetPeriodForDate(lastPeriodStart, firstDayOfWeek, length);

        }

        public static WorkLogPeriod GetPeriodForDate(DateTime date, char firstDayOfWeek, int length)
        {
            var startingDay = GetDayOfWeek(firstDayOfWeek);

            int startOffset = (int)date.DayOfWeek - (int)startingDay;

            if (startOffset < 0)
            {
                startOffset += 7;
            }

            if (startOffset >= length)
            {
                // We're outside of the current work log period
                // Maybe on a weekend or something. Create a new one-off period
                // with just one day...
                return new WorkLogPeriod(date, date);
            }
            else
            {
                var startDate = date.AddDays(-startOffset);
                return new WorkLogPeriod(startDate, length);
            }
        }

        public static char GetDayOfWeek(DayOfWeek dayOfWeek)
        {
            return VALID_WEEKDAYS[((int)dayOfWeek)];
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

        public static bool IsValidDayOfWeek(char dayOfWeek) => VALID_WEEKDAYS.Contains(dayOfWeek);
    }
}
