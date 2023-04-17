namespace JobBoards.WebApplication.Utils;

public static class DateTimeUtils
{
    public static string FormatDateToRelative(this DateTime dateTime)
    {
        TimeSpan timeDifference = DateTime.UtcNow.Subtract(dateTime); // calculate the time difference

        if (timeDifference.TotalMinutes < 1)
        {
            // less than a minute ago
            return "just now";
        }
        else if (timeDifference.TotalMinutes < 60)
        {
            // less than an hour ago
            return $"{(int)timeDifference.TotalMinutes} minutes ago";
        }
        else if (timeDifference.TotalHours < 24)
        {
            // less than a day ago
            return $"{(int)timeDifference.TotalHours} hours ago";
        }
        else
        {
            // more than a day ago
            return dateTime.ToString("MMMM dd, yyyy h:mm tt");
        }
    }
}