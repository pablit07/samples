# coding=utf-8
# RaceAverage class definition
# by paul.kohlhoff@gmail.com#
#
# ʕ •ᴥ•ʔ
#

# constants
MIN_PER_HR = 60
HR_PER_DAY = 24


class RaceAverage:
    """
    used to compute an average finish time for times given after the StartTime
    We are given a list of finish times as a string, where each finish time is formatted as
    hh:mm xM, DAY n
    """

    def __init__(self):
        self.start_time = "08:00 AM, DAY 1"


    def avgMinutes(self, times):

        nTimes = len(times)

        if nTimes < 1 or nTimes > 50:
            raise Exception("Value for times not between 1 and 50 inclusive, yo")

        start_time_in_minutes = self.get_total_min_from_str(self.start_time)
        running_total = 0

        # tally up all the times

        for time in times:
            running_total += (self.get_total_min_from_str(time) - start_time_in_minutes)

        # compute the average using arithmetic mean

        average_time = float(running_total) / float(nTimes)

        return int(round(average_time, 0))

    def get_total_min_from_str(self, time):

        hours = self.get_hours_from_str(time)
        min = self.get_min_from_str(time)
        days = self.get_days_from_str(time)

        total_minutes = min + self.convert_hours_to_min(hours) + self.convert_hours_to_min(
            self.convert_days_to_hours(days))

        return total_minutes

    def get_hours_from_str(self, time):
        hoursBase = int(time[:2])
        amOrPm = time[6:8]

        return self.convert_hours_to_24hr(hoursBase, amOrPm)

    def convert_hours_to_24hr(self, hoursBase, amOrPm):
        if hoursBase == 12 and amOrPm == "AM":
            return 0
        result = hoursBase + (12 if amOrPm == "PM" else 0)
        if result == 24:
            return 12
        return result

    def get_min_from_str(self, time):
        time_ = time[3:5]
        return int(time_)

    def get_days_from_str(self, time):
        return int(time[14:])

    def convert_hours_to_min(self, hours):
        return hours * MIN_PER_HR

    def convert_days_to_hours(self, days):
        return days * HR_PER_DAY




raceAverage = RaceAverage()


print(raceAverage.avgMinutes(["12:00 PM, DAY 1", "12:01 PM, DAY 1"]))
# print(raceAverage.avgMinutes(["02:00 PM, DAY 19", "02:00 PM, DAY 20", "01:58 PM, DAY 20"]))

