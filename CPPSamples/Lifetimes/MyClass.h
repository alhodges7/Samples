#include "stdafx.h"

using namespace System;

ref class MyClass
{
	String ^name;
public:
	MyClass(String ^objectName);
	~MyClass();
	!MyClass();
	void DoSomething();
};
