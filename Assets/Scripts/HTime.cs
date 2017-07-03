using UnityEngine;
using System.Collections;
using System;

public class HTime {

	public int year;
	public int day;

	public int hour;
	public int minute;
	public int second;

	private static HTime lastWorldTimeLiveAdded;

	public HTime () {}

	public HTime (int year,
				int day,
				int hour,
				int minute,
				int second) {
		this.year = year;
		this.day = day;
		this.hour = hour;
		this.minute = minute;
		this.second = second;

		this.ReCunstruct ();
	}

	public HTime (DateTime dateTime) {
		year = dateTime.Year;
		day = dateTime.DayOfYear;
		hour = dateTime.Hour;
		minute = dateTime.Minute;
		second = dateTime.Second;
	}

	void ReCunstruct() {
		minute += second / 60;
		second %= 60;

		hour += minute / 60;
		minute %= 60;

		day += hour / 24;
		hour %= 24;

		int daysInYear = DaysInYear (year);
		while (day / daysInYear != 0) {
			day -= daysInYear * Sign (day);
			year += Sign (day);
			daysInYear = DaysInYear (year);
		}

		int mainSign;

		mainSign = Sign (year);

		if (mainSign == 0) {
			mainSign = Sign (day);
			if (mainSign == 0) {
				mainSign = Sign (hour);
				if (mainSign == 0) {
					mainSign = Sign (minute);
					if (mainSign == 0) {
						mainSign = Sign (second);
					}
				}
			}
		}

		if (Sign (second) != mainSign) {
			minute += Sign (second);
			second -= 60 * Sign (second);
		}
		if (Sign (minute) != mainSign) {
			hour += Sign (minute);
			minute -= 60 * Sign (minute);
		}
		if (Sign (hour) != mainSign) {
			day += Sign (hour);
			hour -= 24 * Sign (hour);
		}
		if (Sign (day) != mainSign) {
			year += Sign (day);
			day -= DaysInYear (year) * Sign (day);
		}

	}

	static int DaysInYear (int year) {
		return (year % 4 == 0) ? 366 : 365;
	}

	int Sign (int i) {
		if (i > 0)
			return 1;
		if (i < 0)
			return -1;
		return 0;
	}

	public static HTime Now {
		get {
			return new HTime (DateTime.Now);
		}
	}

	public static HTime Zero {
		get {
			return new HTime (0, 0, 0, 0, 0);
		}
	}

	public static HTime LastWorldTimeLiveAdded {
		get {
			if (lastWorldTimeLiveAdded == null) {
				lastWorldTimeLiveAdded = new HTime ();
				if (PlayerPrefs.HasKey ("LastWorldTimeScinceLiveAdded_Year"))
					lastWorldTimeLiveAdded.year = PlayerPrefs.GetInt ("LastWorldTimeScinceLiveAdded_Year");
				else
					return null;

				if (PlayerPrefs.HasKey ("LastWorldTimeScinceLiveAdded_Day"))
					lastWorldTimeLiveAdded.day = PlayerPrefs.GetInt ("LastWorldTimeScinceLiveAdded_Day");
				else
					return null;

				if (PlayerPrefs.HasKey ("LastWorldTimeScinceLiveAdded_Hour"))
					lastWorldTimeLiveAdded.hour = PlayerPrefs.GetInt ("LastWorldTimeScinceLiveAdded_Hour");
				else
					return null;

				if (PlayerPrefs.HasKey ("LastWorldTimeScinceLiveAdded_Minute"))
					lastWorldTimeLiveAdded.minute = PlayerPrefs.GetInt ("LastWorldTimeScinceLiveAdded_Minute");
				else
					return null;

				if (PlayerPrefs.HasKey ("LastWorldTimeScinceLiveAdded_Second"))
					lastWorldTimeLiveAdded.second = PlayerPrefs.GetInt ("LastWorldTimeScinceLiveAdded_Second");
				else
					return null;
			}

			return lastWorldTimeLiveAdded;
		}

		set {
			lastWorldTimeLiveAdded = value;
			PlayerPrefs.SetInt ("LastWorldTimeScinceLiveAdded_Year", lastWorldTimeLiveAdded.year);
			PlayerPrefs.SetInt ("LastWorldTimeScinceLiveAdded_Day", lastWorldTimeLiveAdded.day);
			PlayerPrefs.SetInt ("LastWorldTimeScinceLiveAdded_Hour", lastWorldTimeLiveAdded.hour);
			PlayerPrefs.SetInt ("LastWorldTimeScinceLiveAdded_Minute", lastWorldTimeLiveAdded.minute);
			PlayerPrefs.SetInt ("LastWorldTimeScinceLiveAdded_Second", lastWorldTimeLiveAdded.second);
			PlayerPrefs.Save ();
		}
	}

	public static HTime Seconds (int seconds) {
		HTime time = new HTime (0, 0, 0, 0, seconds);
		time.ReCunstruct ();
		return time;
	}

	public static HTime Minutes (int minutes) {
		HTime time = new HTime (0, 0, 0, minutes, 0);
		time.ReCunstruct ();
		return time;
	}

	public static HTime Hours (int hours) {
		HTime time = new HTime (0, 0, hours, 0, 0);
		time.ReCunstruct ();
		return time;
	}

	public static HTime Days (int days) {
		HTime time = new HTime (0, days, 0, 0, 0);
		time.ReCunstruct ();
		return time;
	}

	public static HTime Years (int years) {
		HTime time = new HTime (years, 0, 0, 0, 0);
		return time;
	}

	public int ToInt () {
		return (((((((year * DaysInYear (year)) + day) * 24) + hour) * 60) + minute) * 60) + second;
	}

	public static HTime operator + (HTime a, HTime b) {
		HTime c = new HTime (
			a.year + b.year,
			a.day + b.day,
			a.hour + b.hour,
			a.minute + b.minute,
			a.second + b.second
		);
		c.ReCunstruct ();
		return c;
	}

	public static HTime operator * (HTime a, int i) {
		HTime c = new HTime (
			a.year * i,
			a.day * i,
			a.hour * i,
			a.minute * i,
			a.second * i
		);
		c.ReCunstruct ();
		return c;
	}

	public static HTime operator * (int i, HTime a) {
		return a * i;
	}

	public static int operator / (HTime a, HTime b) {
		long ai = (((((((a.year * DaysInYear (a.year)) + a.day) * 24) + a.hour) * 60) + a.minute) * 60) + a.second;
		long bi = (((((((b.year * DaysInYear (b.year)) + b.day) * 24) + b.hour) * 60) + b.minute) * 60) + b.second;

		return (int) (ai / bi);
	}

	public static HTime operator % (HTime a, HTime b) {
		int ci = a / b;
		HTime c = a - (b * ci);
		return c;
	}

	public static HTime operator - (HTime a) {
		HTime c = new HTime (
			a.year * (-1),
			a.day * (-1),
			a.hour * (-1),
			a.minute * (-1),
			a.second * (-1)
		);
		c.ReCunstruct ();
		return c;
	}

	public static HTime operator - (HTime a, HTime b) {
		HTime c = new HTime (
			a.year - b.year,
			a.day - b.day,
			a.hour - b.hour,
			a.minute - b.minute,
			a.second - b.second
		);
		c.ReCunstruct ();
		return c;
	}

	public static bool operator > (HTime a, HTime b) {
		if (a.year > b.year) {
			return true;
		} else if (a.year < b.year) {
			return false;
		} else {
			if (a.day > b.day) {
				return true;
			} else if (a.day < b.day) {
				return false;
			} else {
				if (a.hour > b.hour) {
					return true;
				} else if (a.hour < b.hour) {
					return false;
				} else {
					if (a.minute > b.minute) {
						return true;
					} else if (a.minute < b.minute) {
						return false;
					} else {
						return a.second > b.second;
					}
				}
			}
		}
	}

	public static bool operator == (HTime a, HTime b) {
		System.Object aObj = a as System.Object;
		System.Object bObj = b as System.Object;

		if (aObj == null ^ bObj == null) {
			return false;
		} else if (aObj == null && bObj == null) {
			return true;
		}

		return (a.second == b.second &&
			a.minute == b.minute &&
			a.hour == b.hour &&
			a.day == b.day &&
			a.year == b.year);
	}

	public static bool operator != (HTime a, HTime b) {
		return !(a == b);
	}

	public static bool operator < (HTime a, HTime b) {
		return (!(a > b) && !(a == b));
	}

	public static bool operator <= (HTime a, HTime b) {
		return ((a < b) || (a == b));
	}
	public static bool operator >= (HTime a, HTime b) {
		return ((a > b) || (a == b));
	}

	public override string ToString () {
		if (this < Zero)
			return "-" + (-this).ToString ();

		String hh, mm, ss;

		if (this.hour > 9)
			hh = this.hour.ToString ();
		else
			hh = "0" + this.hour.ToString ();

		if (this.minute > 9)
			mm = this.minute.ToString ();
		else
			mm = "0" + this.minute.ToString ();

		if (this.second > 9)
			ss = this.second.ToString ();
		else
			ss = "0" + this.second.ToString ();

		return "(year: " + this.year + 
			", day: " + this.day +
			", time: " + hh + ":" + mm + ":" + ss + ")";
	}

	public string MinutesToString () {
		if (this < Zero)
			return "-" + (-this).MinutesToString ();

		String mm, ss;

		if (this.minute > 9)
			mm = this.minute.ToString ();
		else
			mm = "0" + this.minute.ToString ();

		if (this.second > 9)
			ss = this.second.ToString ();
		else
			ss = "0" + this.second.ToString ();

		return mm + ":" + ss;
	}

}
