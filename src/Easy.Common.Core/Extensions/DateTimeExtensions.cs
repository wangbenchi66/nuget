namespace Easy.Common.Core;

public static class DateTimeExtensions
{

    #region 返回当前的毫秒时间戳

    /// <summary>
    /// 返回当前的毫秒时间戳
    /// </summary>
    public static string Msectime()
    {
        long timeTicks = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        return timeTicks.ToString();
    }

    #endregion 返回当前的毫秒时间戳

    #region 剩余多久时间文字描述

    /// <summary>
    /// 剩余多久时间
    /// </summary>
    /// <param name="remainingTime"> </param>
    /// <returns> 文字描述 </returns>
    public static string GetRemainingTime(DateTime remainingTime)
    {
        TimeSpan timeSpan = remainingTime - DateTime.Now;
        var day = timeSpan.Days;
        var hours = timeSpan.Hours;
        var minute = timeSpan.Minutes;
        var seconds = timeSpan.Seconds;
        if (day > 0)
        {
            return day + "天" + hours + "小时" + minute + "分" + seconds + "秒";
        }
        else
        {
            if (hours > 0)
            {
                return hours + "小时" + minute + "分" + seconds + "秒";
            }
            else
            {
                return minute + "分" + seconds + "秒";
            }
        }
    }

    #endregion 剩余多久时间文字描述

    #region 剩余多久时间返回时间类型

    /// <summary>
    /// 剩余多久时间
    /// </summary>
    /// <param name="remainingTime"> </param>
    /// <param name="day"> </param>
    /// <param name="hours"> </param>
    /// <param name="minute"> </param>
    /// <param name="seconds"> </param>
    public static void GetBackTime(DateTime remainingTime, out int day, out int hours, out int minute, out int seconds)
    {
        TimeSpan timeSpan = remainingTime - DateTime.Now;
        day = timeSpan.Days;
        hours = timeSpan.Hours;
        minute = timeSpan.Minutes;
        seconds = timeSpan.Seconds;
    }

    #endregion 剩余多久时间返回时间类型

    #region 计算时间戳剩余多久时间

    /// <summary>
    /// 计算时间戳剩余多久时间
    /// </summary>
    /// <param name="postTime"> 提交时间(要是以前的时间) </param>
    /// <returns> </returns>
    public static string TimeAgo(DateTime postTime)
    {
        //当前时间的时间戳
        var nowtimes = ConvertTicks(DateTime.Now);
        //提交的时间戳
        var posttimes = ConvertTicks(postTime);
        //相差时间戳
        var counttime = nowtimes - posttimes;

        //进行时间转换
        if (counttime <= 60)
        {
            return "刚刚";
        }
        else if (counttime > 60 && counttime <= 120)
        {
            return "1分钟前";
        }
        else if (counttime > 120 && counttime <= 180)
        {
            return "2分钟前";
        }
        else if (counttime > 180 && counttime < 3600)
        {
            return Convert.ToInt32((counttime / 60)) + "分钟前";
        }
        else if (counttime >= 3600 && counttime < 3600 * 24)
        {
            return Convert.ToInt32((counttime / 3600)) + "小时前";
        }
        else if (counttime >= 3600 * 24 && counttime < 3600 * 24 * 2)
        {
            return "昨天";
        }
        else if (counttime >= 3600 * 24 * 2 && counttime < 3600 * 24 * 3)
        {
            return "前天";
        }
        else if (counttime >= 3600 * 24 * 3 && counttime <= 3600 * 24 * 7)
        {
            return Convert.ToInt32((counttime / (3600 * 24))) + "天前";
        }
        else if (counttime >= 3600 * 24 * 7 && counttime <= 3600 * 24 * 30)
        {
            return Convert.ToInt32((counttime / (3600 * 24 * 7))) + "周前";
        }
        else if (counttime >= 3600 * 24 * 30 && counttime <= 3600 * 24 * 365)
        {
            return Convert.ToInt32((counttime / (3600 * 24 * 30))) + "个月前";
        }
        else if (counttime >= 3600 * 24 * 365)
        {
            return Convert.ToInt32((counttime / (3600 * 24 * 365))) + "年前";
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// 时间转换为秒的时间戳
    /// </summary>
    /// <param name="time"> </param>
    /// <returns> </returns>
    private static long ConvertTicks(DateTime time)
    {
        long currentTicks = time.Ticks;
        DateTime dtFrom = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        long currentMillis = (currentTicks - dtFrom.Ticks) / 10000000;  //转换为秒为Ticks/10000000，转换为毫秒Ticks/10000
        return currentMillis;
    }

    #endregion 计算时间戳剩余多久时间

    #region 获取现在是星期几

    /// <summary>
    /// 获取现在是星期几
    /// </summary>
    /// <returns> </returns>
    public static string GetWeek()
    {
        string week = string.Empty;
        switch (DateTime.Now.DayOfWeek)
        {
            case DayOfWeek.Monday:
                week = "周一";
                break;

            case DayOfWeek.Tuesday:
                week = "周二";
                break;

            case DayOfWeek.Wednesday:
                week = "周三";
                break;

            case DayOfWeek.Thursday:
                week = "周四";
                break;

            case DayOfWeek.Friday:
                week = "周五";
                break;

            case DayOfWeek.Saturday:
                week = "周六";
                break;

            case DayOfWeek.Sunday:
                week = "周日";
                break;

            default:
                week = "N/A";
                break;
        }
        return week;
    }

    #endregion 获取现在是星期几

}