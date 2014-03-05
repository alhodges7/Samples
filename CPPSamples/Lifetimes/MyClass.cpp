#include "stdafx.h"
#include "MyClass.h"

using namespace std;


MyClass::MyClass(String ^objectName)
{
	name = objectName;
	Console::WriteLine("Constructor called for {0}", name);
}

MyClass::~MyClass()
{
	Console::WriteLine("Destructor called for {0}", name);
	this->!MyClass();
}

MyClass::!MyClass()
{
	Console::WriteLine("Finalizer called for {0}", name);
}

void MyClass::DoSomething()
{

}
