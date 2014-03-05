// ManagedToUnManaged.cpp : main project file.

#include "stdafx.h"
#include <gcroot.h>

using namespace System;
using namespace System::Runtime::InteropServices;

ref class MClass
{
public:
	int val;
	MClass(int n) : val(n) { }
};

class UClass
{
public:
	gcroot<MClass^> mc;
	UClass(gcroot<MClass^> pmc) : mc(pmc) { }

	int getValue()
	{
		return mc->val;
	}
};

int main(array<System::String ^> ^args)
{
    Console::WriteLine("Testing...");
	MClass ^pm = gcnew MClass(3);
	UClass uc(pm);
	Console::WriteLine("Value is {0}", uc.getValue());
	
	return 0;
}

