#include "stdafx.h"
#include "CreditCardAccount.h"

#using <mscorlib.dll>
using namespace System;

static CreditCardAccount::CreditCardAccount() 
{
	interestRate = 4.5;
	Console::WriteLine("Interest rate: " + interestRate);
}

CreditCardAccount::CreditCardAccount(long number, double limit)
: accountNumber(number), creditLimit(limit), currentBalance(0.0), scheme(nullptr)
{
	numberOfAccounts++;
	Console::WriteLine("This is account number: " + accountNumber);
	return;
}

void CreditCardAccount::SetCreditLimit(double amount)
{
	creditLimit = amount;
}

bool CreditCardAccount::MakePurchase(double amount)
{
	if (currentBalance + amount > creditLimit)
	{
		Console::WriteLine("Insufficient credit - current balance: " + currentBalance);
		return false;
	}
	else
	{
		currentBalance += amount;
		if (currentBalance >= creditLimit / 2)
		{
			if (scheme == nullptr)
			{
				scheme = gcnew LoyaltyScheme();
			}
			else
			{
				scheme->EarnPointsOnAmount(amount);
			}
		}
		return true;
	}
}

void CreditCardAccount::MakeRepayment(double amount)
{
	currentBalance -= amount;
	return;
}

long CreditCardAccount::GetAccountNumber()
{
	return accountNumber;
}

void CreditCardAccount::PrintStatement()
{
	Console::Write("Credit card balance: ");
	Console::WriteLine(currentBalance);
	return;
}

int CreditCardAccount::GetNumberOfAccounts()
{
	return numberOfAccounts;
}

void CreditCardAccount::RedeemLoyaltyPoints()
{
	if (scheme == nullptr)
	{
		Console::WriteLine("Sorry, you do not have a loyalty scheme yet");
	}
	else
	{
		Console::Write("Points available: " + scheme->GetPoints() + ". How many points do you want to redeem? ");
		String ^input = Console::ReadLine();
		int points = Convert::ToInt32(input);
		scheme->RedeemPoints(points);
		Console::WriteLine("Points remaining: " + scheme->GetPoints());
	}
}