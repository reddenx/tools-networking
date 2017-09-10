using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Reflection.Tests
{
    public class InputClass
    {
        public string Input;

        public InputClass(string input)
        {
            Input = input;
        }
    }

    public class OutputClass
    {
        public string Output;

        public OutputClass(string output)
        {
            Output = output;
        }
    }

    public interface ITestInterface
    {
        int ParameterPrimitiveReadonly { get; }
        float ParameterPrimitiveReadWrite { get; set; }
        OutputClass ParameterComplexReadonly { get; }
        OutputClass ParameterComplexReadWrite { get; set; }

        void MethodVoidVoid();
        void MethodVoidSinglePrimitive(int input);
        void MethodVoidManyPrimitive(int input1, int input2, float input3);
        void MethodVoidSingleClass(InputClass input1);
        void MethodVoidManyClass(InputClass input1, InputClass input2, String input3);
        void MethodVoidMixed(InputClass input1, int input2, InputClass input3);
        void MethodVoidMixed2(int input1, InputClass input2, int input3);

        int MethodPrimitiveVoid();
        int MethodPrimitiveSinglePrimitive(int input);
        int MethodPrimitiveManyPrimitive(int input1, int input2, float input3);
        int MethodPrimitiveSingleClass(InputClass input1);
        int MethodPrimitiveManyClass(InputClass input1, InputClass input2, String input3);
        int MethodPrimitiveMixed(InputClass input1, int input2, InputClass input3);
        int MethodPrimitiveMixed2(int input1, InputClass input2, int input3);

        OutputClass MethodOverloadMethodTest(InputClass input1, string input2);
        OutputClass MethodOverloadMethodTest(InputClass input1, string input2, int input3);

        OutputClass MethodClassVoid();
        OutputClass MethodClassSinglePrimitive(int input);
        OutputClass MethodClassManyPrimitive(int input1, int input2, float input3);
        OutputClass MethodClassSingleClass(InputClass input1);
        OutputClass MethodClassManyClass(InputClass input1, InputClass input2, String input3);
        OutputClass MethodClassMixed(InputClass input1, int input2, InputClass input3);
        OutputClass MethodClassMixed2(int input1, InputClass input2, int input3);
    }

    public interface IAmAGenericBastard<T>
    {
        T GetThings();
    }

    public interface IHaveAGenericBastard
    {
        T GetThings<T>();
    }

    //public class TestClassImplementation : ITestInterface
    //{
    //    public OutputClass ComplexReadonly
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public OutputClass ComplexReadWrite
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }

    //        set
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public int PrimitiveReadonly
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public float PrimitiveReadWrite
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }

    //        set
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public OutputClass ClassManyClass(InputClass input1, InputClass input2, string input3)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public OutputClass ClassManyPrimitive(int input1, int input2, float input3)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public OutputClass ClassMixed(InputClass input1, int input2, InputClass input3)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public OutputClass ClassMixed2(int input1, InputClass input2, int input3)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public OutputClass ClassSingleClass(InputClass input1)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public OutputClass ClassSinglePrimitive(int input)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public OutputClass ClassVoid()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public int PrimitiveManyClass(InputClass input1, InputClass input2, string input3)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public int PrimitiveManyPrimitive(int input1, int input2, float input3)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public int PrimitiveMixed(InputClass input1, int input2, InputClass input3)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public int PrimitiveMixed2(int input1, InputClass input2, int input3)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public int PrimitiveSingleClass(InputClass input1)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public int PrimitiveSinglePrimitive(int input)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public int PrimitiveVoid()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void VoidManyClass(InputClass input1, InputClass input2, string input3)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void VoidManyPrimitive(int input1, int input2, float input3)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void VoidMixed(InputClass input1, int input2, InputClass input3)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void VoidMixed2(int input1, InputClass input2, int input3)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void VoidSingleClass(InputClass input1)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void VoidSinglePrimitive(int input)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void VoidVoid()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
