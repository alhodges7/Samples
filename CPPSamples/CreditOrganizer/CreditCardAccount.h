#include "LoyaltyScheme.h"

ref class CreditCardAccount
{
public:
	static CreditCardAccount();
	CreditCardAccount(long accountNum, double creditLim);
	static int GetNumberOfAccounts();
	void SetCreditLimit(double amount);
	bool MakePurchase(double amount);
	void MakeRepayment(double amount);
	void PrintStatement();
	long GetAccountNumber();
	void RedeemLoyaltyPoints();
private:
	static int numberOfAccounts = 0;
	static double interestRate;
	initonly long accountNumber;
	double currentBalance;
	double creditLimit;
	LoyaltyScheme ^scheme;
};