#include "stdafx.h"
#include "MyClass.h"

using namespace System;

int main()
{
	MyClass ^m1 = gcnew MyClass("m1");
	m1->DoSomething();
	delete m1;

	MyClass m2("m2");
	m2.DoSomething();

    Console::WriteLine();
	Console::WriteLine("End of program");
	Console::WriteLine();
	return 0;
}
