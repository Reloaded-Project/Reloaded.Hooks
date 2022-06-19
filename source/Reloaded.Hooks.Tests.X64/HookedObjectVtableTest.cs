using System;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Helpers;
using Reloaded.Hooks.Tests.Shared;
using Reloaded.Hooks.Tools;
using Xunit;

namespace Reloaded.Hooks.Tests.X64
{
    public class HookedObjectVtableTest : IDisposable
    {
        private NativeCalculator _nativeCalculator;

        private IHook<NativeCalculator.AddFunction> _addHook;
        private IHook<NativeCalculator.SubtractFunction> _subHook;
        private IHook<NativeCalculator.DivideFunction> _divideHook;
        private IHook<NativeCalculator.MultiplyFunction> _multiplyHook;

        public HookedObjectVtableTest()
        {
            _nativeCalculator = new NativeCalculator();
        }

        public void Dispose()
        {
            _nativeCalculator?.Dispose();
        }

        [Fact]
        public void TestFunctionPtr()
        {
            // Note: All delegates are the same; this test will need to be changed if they ever change.
            FunctionPtr<NativeCalculator.AddFunction> functionPtr = new FunctionPtr<NativeCalculator.AddFunction>((ulong) _nativeCalculator.VTable);

            for (int x = 1; x < 100; x++)
            {
                for (int y = 100; y > 0; y--)
                {
                    int add = x + y;
                    int subtract = x - y;
                    int multiply = x * y;
                    int divide = x / y;

                    Assert.Equal(add, functionPtr[(int) NativeCalculator.VTableFunctions.Add](x, y));
                    Assert.Equal(subtract, functionPtr[(int) NativeCalculator.VTableFunctions.Subtract](x, y));
                    Assert.Equal(multiply, functionPtr[(int) NativeCalculator.VTableFunctions.Multiply](x, y));
                    Assert.Equal(divide, functionPtr[(int) NativeCalculator.VTableFunctions.Divide](x, y));
                }
            }
        }

        [Fact]
        public unsafe void TestInternalVTableCallAfterVTablePointerIsHooked()
        {
            // HookedObjectVirtualFunctionTable only supports hooking objects
            // where the vtable pointer is at [objectbaseadress]
            // to test this, we create a pointer to our vtable.
            // this serves as a fake object, which we can hook
            nuint vTableOriginal = _nativeCalculator.VTable;
            IntPtr fakeObject = new IntPtr(&vTableOriginal);

            // we hook the vtable pointer(fakeObject), this changes the vtable pointer but does not hook any functions yet
            var vTableHook = HookedObjectVirtualFunctionTable.FromObject(fakeObject.ToUnsigned(), Enum.GetNames(typeof(NativeCalculator.VTableFunctions)).Length); ;
         
            // Setup calling functions.
            var addFunction = vTableHook.CreateWrapperFunction<NativeCalculator.AddFunction>((int) NativeCalculator.VTableFunctions.Add);
            var subtractFunction = vTableHook.CreateWrapperFunction<NativeCalculator.SubtractFunction>((int) NativeCalculator.VTableFunctions.Subtract);
            var multiplyFunction = vTableHook.CreateWrapperFunction<NativeCalculator.MultiplyFunction>((int) NativeCalculator.VTableFunctions.Multiply);
            var divideFunction = vTableHook.CreateWrapperFunction<NativeCalculator.DivideFunction>((int) NativeCalculator.VTableFunctions.Divide);

            // This test only tests if calling functions using HookedObjectVirtualFunctionTable still works
            // We need another test to check if -others- can still call the vtable functions without prolems
            // This is done in TestExternalVTableCallAfterVTablePointerIsHooked and TestExternalVTableCallAfterFunctionHook
            // Test Calling after we hook the vtable pointer using our HookedObjectVirtualFunctionTable
            // This should not modify any results
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
        public unsafe void TestExternalVTableCallAfterVTablePointerIsHooked()
        {
            // HookedObjectVirtualFunctionTable only supports hooking objects
            // where the vtable pointer is at [objectbaseadress]
            // to test this, we create a pointer to our vtable.
            // this serves as a fake object, which we can hook
            nuint vTableOriginal = _nativeCalculator.VTable;
            IntPtr fakeObject = new IntPtr(&vTableOriginal);
            // We hook the vtable pointer(fakeObject), this changes the vtable pointer but does not hook any functions yet
            // Checking if this corrupts anything is done in TestVTableHookExternalCallBeforeHook
            var vTableHook = HookedObjectVirtualFunctionTable.FromObject(fakeObject.ToUnsigned(), Enum.GetNames(typeof(NativeCalculator.VTableFunctions)).Length); ;

            // We create a VirtualFunctionTable from our fakeObject
            // Calling the functions should call our hooks

            var vTable = VirtualFunctionTable.FromObject(fakeObject.ToUnsigned(), Enum.GetNames(typeof(NativeCalculator.VTableFunctions)).Length);
            var addFunction = vTable.CreateWrapperFunction<NativeCalculator.AddFunction>((int)NativeCalculator.VTableFunctions.Add);
            var subtractFunction = vTable.CreateWrapperFunction<NativeCalculator.SubtractFunction>((int)NativeCalculator.VTableFunctions.Subtract);
            var multiplyFunction = vTable.CreateWrapperFunction<NativeCalculator.MultiplyFunction>((int)NativeCalculator.VTableFunctions.Multiply);
            var divideFunction = vTable.CreateWrapperFunction<NativeCalculator.DivideFunction>((int)NativeCalculator.VTableFunctions.Divide);
            // Test Calling after we hook the vtable pointer using an 'external' class (VirtualFunctionTable)
            // This should not modify any results
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
        public unsafe void TestExternalVTableCallAfterFunctionHook()
        {
            int addHook(int a, int b) { return _addHook.OriginalFunction(a, b) + 1; }
            int subHook(int a, int b) { return _subHook.OriginalFunction(a, b) - 1; }
            int mulHook(int a, int b) { return _multiplyHook.OriginalFunction(a, b) * 2; }
            int divHook(int a, int b) { return _divideHook.OriginalFunction(a, b) * 2; }

            // HookedObjectVirtualFunctionTable only supports hooking objects
            // where the vtable pointer is at [objectbaseadress]
            // to test this, we create a pointer to our vtable.
            // this serves as a fake object, which we can hook
            nuint vTableOriginal = _nativeCalculator.VTable;
            IntPtr fakeObject = new IntPtr(&vTableOriginal);

            // We hook the vtable pointer(fakeObject), this changes the vtable pointer but does not hook any functions yet
            // Checking if this corrupts anything is done in TestVTableCallsAfterHookingVTablePointer
            var vTableHook = HookedObjectVirtualFunctionTable.FromObject(fakeObject.ToUnsigned(), Enum.GetNames(typeof(NativeCalculator.VTableFunctions)).Length);;
            _addHook = vTableHook.CreateFunctionHook<NativeCalculator.AddFunction>((int)NativeCalculator.VTableFunctions.Add, addHook).Activate();
            _subHook = vTableHook.CreateFunctionHook<NativeCalculator.SubtractFunction>((int)NativeCalculator.VTableFunctions.Subtract, subHook).Activate();
            _multiplyHook = vTableHook.CreateFunctionHook<NativeCalculator.MultiplyFunction>((int)NativeCalculator.VTableFunctions.Multiply, mulHook).Activate();
            _divideHook = vTableHook.CreateFunctionHook<NativeCalculator.DivideFunction>((int)NativeCalculator.VTableFunctions.Divide, divHook).Activate();

            // We create a VirtualFunctionTable from our fakeObject
            // Calling the functions should call our hooks
            var vTable = VirtualFunctionTable.FromObject(fakeObject.ToUnsigned(), Enum.GetNames(typeof(NativeCalculator.VTableFunctions)).Length);
            var addFunction = vTable.CreateWrapperFunction<NativeCalculator.AddFunction>((int)NativeCalculator.VTableFunctions.Add);
            var subtractFunction = vTable.CreateWrapperFunction<NativeCalculator.SubtractFunction>((int)NativeCalculator.VTableFunctions.Subtract);
            var multiplyFunction = vTable.CreateWrapperFunction<NativeCalculator.MultiplyFunction>((int)NativeCalculator.VTableFunctions.Multiply);
            var divideFunction = vTable.CreateWrapperFunction<NativeCalculator.DivideFunction>((int)NativeCalculator.VTableFunctions.Divide);

            // Test Calling after we hook the vtable function pointers using an 'external' class (VirtualFunctionTable)
            // This should not modify any results
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

        [Fact]
        public unsafe void TestExternalVTableCallAfterFunctionHookButWithDirectCall()
        {
            int addHook(int a, int b) { return _addHook.OriginalFunction(a, b) + 1; }
            int subHook(int a, int b) { return _subHook.OriginalFunction(a, b) - 1; }
            int mulHook(int a, int b) { return _multiplyHook.OriginalFunction(a, b) * 2; }
            int divHook(int a, int b) { return _divideHook.OriginalFunction(a, b) * 2; }

            // HookedObjectVirtualFunctionTable only supports hooking objects
            // where the vtable pointer is at [objectbaseadress]
            // to test this, we create a pointer to our vtable.
            // this serves as a fake object, which we can hook
            nuint vTableOriginal = _nativeCalculator.VTable;
            IntPtr fakeObject = new IntPtr(&vTableOriginal);

            // We hook the vtable pointer(fakeObject), this changes the vtable pointer but does not hook any functions yet
            // Checking if this corrupts anything is done in TestVTableCallsAfterHookingVTablePointer
            var vTableHook = HookedObjectVirtualFunctionTable.FromObject(fakeObject.ToUnsigned(), Enum.GetNames(typeof(NativeCalculator.VTableFunctions)).Length); ;

            _addHook = vTableHook.CreateFunctionHook<NativeCalculator.AddFunction>((int)NativeCalculator.VTableFunctions.Add, addHook).Activate();
            _subHook = vTableHook.CreateFunctionHook<NativeCalculator.SubtractFunction>((int)NativeCalculator.VTableFunctions.Subtract, subHook).Activate();
            _multiplyHook = vTableHook.CreateFunctionHook<NativeCalculator.MultiplyFunction>((int)NativeCalculator.VTableFunctions.Multiply, mulHook).Activate();
            _divideHook = vTableHook.CreateFunctionHook<NativeCalculator.DivideFunction>((int)NativeCalculator.VTableFunctions.Divide, divHook).Activate();

            // In this test we access our vtable directly
            // As the vtable itself is never changed, this should not hook any functions
            var vTable = VirtualFunctionTable.FromAddress(_nativeCalculator.VTable, Enum.GetNames(typeof(NativeCalculator.VTableFunctions)).Length);

            var addFunction = vTable.CreateWrapperFunction<NativeCalculator.AddFunction>((int)NativeCalculator.VTableFunctions.Add);
            var subtractFunction = vTable.CreateWrapperFunction<NativeCalculator.SubtractFunction>((int)NativeCalculator.VTableFunctions.Subtract);
            var multiplyFunction = vTable.CreateWrapperFunction<NativeCalculator.MultiplyFunction>((int)NativeCalculator.VTableFunctions.Multiply);
            var divideFunction = vTable.CreateWrapperFunction<NativeCalculator.DivideFunction>((int)NativeCalculator.VTableFunctions.Divide);

            // Test Calling after we hook the vtable function pointers using an 'external' class (VirtualFunctionTable)
            // This should not modify any results
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
    }
}
