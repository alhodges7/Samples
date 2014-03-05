// Hello.cpp : main project file.

#include "stdafx.h"

using namespace System;

int GetYear();
int GetMonth();
int GetDay(int year, int month);
void DisplayDate(int year, int month, int day);

int main()
{
	Console::WriteLine("Welcome to your calendar assistant");
	Console::WriteLine("\nPlease enter a date");
	int year = GetYear();
	int month = GetMonth();
	int day = GetDay(year, month);

	if (month >= 1 && month <= 12 && day >= 1 && day <= 31)
	{
		DisplayDate(year, month, day);
	}
	else
	{
		Console::WriteLine("Invalid date");
	}
	Console::WriteLine("\nThe end\n");

	return 0;
}

int GetYear()
{
	Console::Write("Year? ");
	return Convert::ToInt32(Console::ReadLine());
}

int GetMonth()
{
	Console::Write("Month? ");
	return Convert::ToInt32(Console::ReadLine());
}

int GetDay(int year, int month)
{
	int day, maxDay;
	if (month == 4 || month == 6 || month == 9 || month == 11)
	{
		maxDay = 30;
	}
	else if (month == 2)
	{
		bool isLeapYear = (year % 4 == 0 && year % 100 != 0 || year % 100 == 0);
		if (isLeapYear)
		{
			maxDay = 29;
		}
		else
		{
			maxDay = 28;
		}
	}
	else
	{
		maxDay = 31;
	}
	do
	{
		Console::Write("Day [1 to " + maxDay + "]? ");
		day = Convert::ToInt32(Console::ReadLine());
	} while (day < 1 || day > maxDay);

	return day;
}

void DisplayDate(int year, int month, int day)
{
	Console::WriteLine("\nThis is the date you entered:");
	Console::Write(year + "-");
	switch (month)
	{
	case 1: Console::Write("January"); break;
	case 2: Console::Write("February"); break;
	case 3: Console::Write("March"); break;
	case 4: Console::Write("April"); break;
	case 5: Console::Write("May"); break;
	case 6: Console::Write("June"); break;
	case 7: Console::Write("July"); break;
	case 8: Console::Write("August"); break;
	case 9: Console::Write("September"); break;
	case 10: Console::Write("October"); break;
	case 11: Console::Write("November"); break;
	case 12: Console::Write("December"); break;
	}
	Console::Write("-" + day + "\n");
}