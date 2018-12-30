using System;
using Reloaded.Hooks.Tests.Shared;
using Reloaded.Hooks.Tools;
using Reloaded.Hooks.X64;
using Xunit;

namespace Reloaded.Hooks.Tests.X64
{
    public class VTableTest : IDisposable
    {
        private Calculator _calculator;

        private IHook<Calculator.AddFunction> _addHook;
        private IHook<Calculator.SubtractFunction> _subHook;
        private IHook<Calculator.DivideFunction> _divideHook;
        private IHook<Calculator.MultiplyFunction> _multiplyHook;

        public VTableTest()
        {
            _calculator = new Calculator();
        }

        public void Dispose()
        {
            _calculator?.Dispose();
        }

        [Fact]
        public void TestFunctionPtr()
        {
            // Note: All delegates are the same; this test will need to be changed if they ever change.
            FunctionPtr<Calculator.AddFunction> functionPtr = new FunctionPtr<Calculator.AddFunction>((ulong) _calculator.VTable);

            for (int x = 1; x < 100; x++)
            {
                for (int y = 100; y > 0; y--)
                {
                    int add = x + y;
                    int subtract = x - y;
                    int multiply = x * y;
                    int divide = x / y;

                    Assert.Equal(add, functionPtr[(int) Calculator.VTableFunctions.Add](x, y));
                    Assert.Equal(subtract, functionPtr[(int) Calculator.VTableFunctions.Subtract](x, y));
                    Assert.Equal(multiply, functionPtr[(int) Calculator.VTableFunctions.Multiply](x, y));
                    Assert.Equal(divide, functionPtr[(int) Calculator.VTableFunctions.Divide](x, y));
                }
            }
        }

        [Fact]
        public void TestVTableCall()
        {
            var vTable = VirtualFunctionTable.FromAddress(_calculator.VTable, Enum.GetNames(typeof(Calculator.VTableFunctions)).Length);

            // Setup calling functions.
            var addFunction = vTable.CreateWrapperFunction<Calculator.AddFunction>((int) Calculator.VTableFunctions.Add);
            var subtractFunction = vTable.CreateWrapperFunction<Calculator.SubtractFunction>((int) Calculator.VTableFunctions.Subtract);
            var multiplyFunction = vTable.CreateWrapperFunction<Calculator.MultiplyFunction>((int) Calculator.VTableFunctions.Multiply);
            var divideFunction = vTable.CreateWrapperFunction<Calculator.DivideFunction>((int) Calculator.VTableFunctions.Divide);

            // Test Calling
            for (int x = 1; x < 100; x++)
            {
                for (int y = 100; y > 0; y--)
                {
                    int add = x + y;
                    int subtract = x - y;
                    int multiply = x * y;
                    int divide = x / y;

                    Assert.Equal(add, addFunction(x, y));
                    Assert.Equal(subtract, subtractFunction(x, y));
                    Assert.Equal(multiply, multiplyFunction(x, y));
                    Assert.Equal(divide, divideFunction(x, y));
                }
            }
        }

        [Fact]
        public void TestVTableHook()
        {
            var vTable = VirtualFunctionTable.FromAddress(_calculator.VTable, Enum.GetNames(typeof(Calculator.VTableFunctions)).Length);
            
            var addFunction = vTable.CreateWrapperFunction<Calculator.AddFunction>((int)Calculator.VTableFunctions.Add);
            var subtractFunction = vTable.CreateWrapperFunction<Calculator.SubtractFunction>((int)Calculator.VTableFunctions.Subtract);
            var multiplyFunction = vTable.CreateWrapperFunction<Calculator.MultiplyFunction>((int)Calculator.VTableFunctions.Multiply);
            var divideFunction = vTable.CreateWrapperFunction<Calculator.DivideFunction>((int)Calculator.VTableFunctions.Divide);

            int addHook(int a, int b) { return _addHook.OriginalFunction(a, b) + 1; }
            int subHook(int a, int b) { return _subHook.OriginalFunction(a, b) - 1; }
            int mulHook(int a, int b) { return _multiplyHook.OriginalFunction(a, b) * 2; }
            int divHook(int a, int b) { return _divideHook.OriginalFunction(a, b) * 2; }

            _addHook = vTable.CreateFunctionHook<Calculator.AddFunction>((int)Calculator.VTableFunctions.Add, addHook).Activate();
            _subHook = vTable.CreateFunctionHook<Calculator.SubtractFunction>((int)Calculator.VTableFunctions.Subtract, subHook).Activate();
            _multiplyHook = vTable.CreateFunctionHook<Calculator.MultiplyFunction>((int)Calculator.VTableFunctions.Multiply, mulHook).Activate();
            _divideHook = vTable.CreateFunctionHook<Calculator.DivideFunction>((int)Calculator.VTableFunctions.Divide, divHook).Activate();

            for (int x = 1; x < 100; x++)
            {
                for (int y = 100; y > 0; y--)
                {
                    int add = (x + y) + 1;
                    int subtract = (x - y) - 1;
                    int multiply = (x * y) * 2;
                    int divide = (x / y) * 2;

                    Assert.Equal(add, addFunction(x, y));
                    Assert.Equal(subtract, subtractFunction(x, y));
                    Assert.Equal(multiply, multiplyFunction(x, y));
                    Assert.Equal(divide, divideFunction(x, y));
                }
            }
        }
    }
}
