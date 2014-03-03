#include "stdafx.h"
#include "LoyaltyScheme.h"

#using <mscorlib.dll>
using namespace System;

LoyaltyScheme::LoyaltyScheme()
: totalPoints(0)
{
	Console::WriteLine("Congratulations, you now qualify for"
		" bonus points");
}

void LoyaltyScheme::EarnPointsOnAmount(double amountSpent)
{
	int points = (int)(amountSpent / 10);
	totalPoints += points;
	Console::WriteLine("New bonus points earned: " + points);
}

void LoyaltyScheme::RedeemPoints(int points)
{
	if (points < totalPoints)
	{
		totalPoints -= points;
	}
	else
	{
		totalPoints = 0;
	}
}

int LoyaltyScheme::GetPoints()
{
	return totalPoints;
}