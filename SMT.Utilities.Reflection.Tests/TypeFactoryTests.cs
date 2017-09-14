using System;
using SMT.Utilities.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SMT.Utilities.Reflection.Tests
{
    [TestClass]
    public class TypeFactoryTests
    {
        [TestMethod]
        public void SetImplementationSignatureMismatch()
        {
            var generationResult = TypeFactory.BuildType<ITestInterface>();

            try
            {
                generationResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodVoidVoid), new[]
                {
                    typeof(int),
                },
                o => { return null; });
                Assert.Fail("did not throw an exception when it should have");
            }
            catch (ArgumentException) { }
            catch (Exception e)
            {
                Assert.Fail($"wrong type of exception thrown: {e.GetType()}");
            }
        }

        [TestMethod]
        public void SetImplementationSignatureMismatchGeneric()
        {
            var generationResult = TypeFactory.BuildType<ITestInterface>();

            try
            {
                generationResult.Interceptor.SetMethodImplementation<Func<string, string, int, string>>(nameof(ITestInterface.MethodVoidSingleClass),
                (s, s2, i) =>
                {
                    return "null?";
                });
                Assert.Fail("did not throw an exception when it should have");
            }
            catch (ArgumentException) { }
            catch (Exception e)
            {
                Assert.Fail($"wrong type of exception thrown: {e.GetType()}");
            }
        }

        [TestMethod]
        public void DidNotSetImplementationFunc()
        {
            var generationResult = TypeFactory.BuildType<ITestInterface>();

            try
            {
                generationResult.InterceptedInstance.MethodClassVoid();
                Assert.Fail("did not throw an exception when it should have");
            }
            catch (NotImplementedException) { }
            catch (Exception e)
            {
                Assert.Fail($"wrong type of exception thrown: {e.GetType()}");
            }
        }

        [TestMethod]
        public void DidNotSetImplementationAction()
        {
            var generationResult = TypeFactory.BuildType<ITestInterface>();

            try
            {
                generationResult.InterceptedInstance.MethodVoidVoid();
                Assert.Fail("did not throw an exception when it should have");
            }
            catch (NotImplementedException) { }
            catch (Exception e)
            {
                Assert.Fail($"wrong type of exception thrown: {e.GetType()}");
            }
        }

        [TestMethod]
        public void NoGenericTypesEnforced()
        {
            try
            {
                var result = TypeFactory.BuildType<IAmAGenericBastard<string>>();
                Assert.Fail("should have preveneted the masking of generic types");
            }
            catch (NotImplementedException) { }
            catch (Exception e)
            {
                Assert.Fail($"wrong type of exception thrown: {e.GetType()}");
            }
        }

        [TestMethod]
        public void NoGenericTypesEnforcedInMethods()
        {
            try
            {
                var result = TypeFactory.BuildType<IHaveAGenericBastard>();
                Assert.Fail("should have preveneted the masking of generic types");
            }
            catch (NotImplementedException) { }
            catch (Exception e)
            {
                Assert.Fail($"wrong type of exception thrown: {e.GetType()}");
            }
        }

        [TestMethod]
        public void OnlyConsumeInterfaceTypes()
        {
            try
            {
                var result = TypeFactory.BuildType<InputClass>();
                Assert.Fail("should have prevented the masking of concrete types");
            }
            catch (NotImplementedException) { }
            catch (Exception e)
            {
                Assert.Fail($"wrong type of exception thrown: {e.GetType()}");
            }
        }

        #region RepetativeMethodTesting

        /* 
         * yes this looks crazy, but the repetative copypasta looking code deliberately sets scope/stack correctly.
         * yes this could easily be done with a loop over reflected methods but that wouldn't accomplish the goal of these tests
         * DO NOT REFACTOR
         */

        [TestMethod]
        public void MethodVoidVoid()
        {
            //test each and every method, ensure it was set
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodVoidVoid), new Type[]
            {
            },
            (o) =>
            {
                called = true;
                return null;
            });
            generatedResult.InterceptedInstance.MethodVoidVoid();
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void MethodVoidSinglePrimitive()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var input1 = RandInt();
            var called = false;
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodVoidSinglePrimitive), new Type[]
            {
                typeof(int)
            },
            (o) =>
            {
                Assert.AreEqual(o[0], input1);
                called = true;
                return null;
            });
            generatedResult.InterceptedInstance.MethodVoidSinglePrimitive(input1);
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void MethodVoidManyPrimitive()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInt();
            var input2 = RandInt();
            var input3 = RandFloat();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodVoidManyPrimitive), new Type[]
            {
                typeof(int),
                typeof(int),
                typeof(float)
            },
            (o) =>
            {
                Assert.AreEqual(o[0], input1);
                Assert.AreEqual(o[1], input2);
                Assert.AreEqual(o[2], input3);
                called = true;
                return null;
            });
            generatedResult.InterceptedInstance.MethodVoidManyPrimitive(input1, input2, input3);
            Assert.IsTrue(called);

        }

        [TestMethod]
        public void MethodVoidSingleClass()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInputClass();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodVoidSingleClass), new Type[]
            {
                typeof(InputClass)
            },
            (o) =>
            {
                Assert.AreEqual((o[0] as InputClass).Input, input1.Input);
                called = true;
                return null;
            });
            generatedResult.InterceptedInstance.MethodVoidSingleClass(input1);
            Assert.IsTrue(called);

        }

        [TestMethod]
        public void MethodVoidManyClass()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInputClass();
            var input2 = RandInputClass();
            var input3 = RandString();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodVoidManyClass), new Type[]
            {
                typeof(InputClass),
                typeof(InputClass),
                typeof(string)
            },
            (o) =>
            {
                Assert.AreEqual((o[0] as InputClass).Input, input1.Input);
                Assert.AreEqual((o[1] as InputClass).Input, input2.Input);
                Assert.AreEqual((o[2] as string), input3);
                called = true;
                return null;
            });
            generatedResult.InterceptedInstance.MethodVoidManyClass(input1, input2, input3);
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void MethodVoidMixed()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();
            var called = false;
            var input1 = RandInputClass();
            var input2 = RandInt();
            var input3 = RandInputClass();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodVoidMixed), new Type[]
            {
                typeof(InputClass),
                typeof(int),
                typeof(InputClass),
            },
            (o) =>
            {
                Assert.AreEqual((o[0] as InputClass).Input, input1.Input);
                Assert.AreEqual((o[1]), input2);
                Assert.AreEqual((o[2] as InputClass).Input, input3.Input);
                called = true;
                return null;
            });
            generatedResult.InterceptedInstance.MethodVoidMixed(input1, input2, input3);
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void MethodVoidMixed2()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInt();
            var input2 = RandInputClass();
            var input3 = RandInt();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodVoidMixed2), new Type[]
            {
                typeof(int),
                typeof(InputClass),
                typeof(int),
            },
                (o) =>
                {
                    Assert.AreEqual(o[0], input1);
                    Assert.AreEqual((o[1] as InputClass).Input, input2.Input);
                    Assert.AreEqual(o[2], input3);
                    called = true;
                    return null;
                });
            generatedResult.InterceptedInstance.MethodVoidMixed2(input1, input2, input3);
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void MethodPrimitiveVoid()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var output = RandInt();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodPrimitiveVoid), new Type[]
            {
            },
            (o) =>
            {
                called = true;
                return output;
            });
            var result = generatedResult.InterceptedInstance.MethodPrimitiveVoid();
            Assert.IsTrue(called);
            Assert.AreEqual(output, result);
        }


        [TestMethod]
        public void MethodPrimitiveSinglePrimitive()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInt();
            var output = RandInt();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodPrimitiveSinglePrimitive), new Type[]
            {
                typeof(int)
            },
            (o) =>
            {
                Assert.AreEqual(o[0], input1);
                called = true;
                return output;
            });
            var result = generatedResult.InterceptedInstance.MethodPrimitiveSinglePrimitive(input1);
            Assert.IsTrue(called);
            Assert.AreEqual(output, result);
        }

        [TestMethod]
        public void MethodPrimitiveManyPrimitive()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInt();
            var input2 = RandInt();
            var input3 = RandFloat();
            var output = RandInt();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodPrimitiveManyPrimitive), new Type[]
            {
                typeof(int),
                typeof(int),
                typeof(float),
            },
            (o) =>
            {
                Assert.AreEqual(o[0], input1);
                Assert.AreEqual(o[1], input2);
                Assert.AreEqual(o[2], input3);
                called = true;
                return output;
            });
            var result = generatedResult.InterceptedInstance.MethodPrimitiveManyPrimitive(input1, input2, input3);
            Assert.IsTrue(called);
            Assert.AreEqual(output, result);
        }

        [TestMethod]
        public void MethodPrimitiveSingleClass()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInputClass();
            var output = RandInt();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodPrimitiveSingleClass), new Type[]
            {
                typeof(InputClass),
            },
            (o) =>
            {
                Assert.AreEqual((o[0] as InputClass).Input, input1.Input);
                called = true;
                return output;
            });
            var result = generatedResult.InterceptedInstance.MethodPrimitiveSingleClass(input1);
            Assert.IsTrue(called);
            Assert.AreEqual(output, result);
        }

        [TestMethod]
        public void MethodPrimitiveManyClass()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInputClass();
            var input2 = RandInputClass();
            var input3 = RandString();
            var output = RandInt();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodPrimitiveManyClass), new Type[]
            {
                typeof(InputClass),
                typeof(InputClass),
                typeof(string),
            },
            (o) =>
            {
                Assert.AreEqual((o[0] as InputClass).Input, input1.Input);
                Assert.AreEqual((o[1] as InputClass).Input, input2.Input);
                Assert.AreEqual((o[2] as string), input3);
                called = true;
                return output;
            });
            var result = generatedResult.InterceptedInstance.MethodPrimitiveManyClass(input1, input2, input3);
            Assert.IsTrue(called);
            Assert.AreEqual(output, result);
        }

        [TestMethod]
        public void MethodPrimitiveMixed()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInputClass();
            var input2 = RandInt();
            var input3 = RandInputClass();
            var output = RandInt();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodPrimitiveMixed), new Type[]
            {
                typeof(InputClass),
                typeof(int),
                typeof(InputClass),
            },
            (o) =>
            {
                Assert.AreEqual((o[0] as InputClass).Input, input1.Input);
                Assert.AreEqual(o[1], input2);
                Assert.AreEqual((o[2] as InputClass).Input, input3.Input);
                called = true;
                return output;
            });
            var result = generatedResult.InterceptedInstance.MethodPrimitiveMixed(input1, input2, input3);
            Assert.IsTrue(called);
            Assert.AreEqual(output, result);
        }


        [TestMethod]
        public void MethodPrimitiveMixed2()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInt();
            var input2 = new InputClass(Guid.NewGuid().ToString());
            var input3 = RandInt();
            var output = RandInt();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodPrimitiveMixed2), new Type[]
            {
                typeof(int),
                typeof(InputClass),
                typeof(int),
            },
            (o) =>
            {
                Assert.AreEqual(o[0], input1);
                Assert.AreEqual((o[1] as InputClass).Input, input2.Input);
                Assert.AreEqual(o[2], input3);
                called = true;
                return output;
            });
            var result = generatedResult.InterceptedInstance.MethodPrimitiveMixed2(input1, input2, input3);
            Assert.IsTrue(called);
            Assert.AreEqual(output, result);
        }

        [TestMethod]
        public void MethodOverloadMethodTest()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInputClass();
            var input2 = RandString();
            var output = RandInt();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodOverloadMethodTest), new Type[]
            {
                typeof(InputClass),
                typeof(string),
            },
            (o) =>
            {
                Assert.AreEqual((o[0] as InputClass).Input, input1.Input);
                Assert.AreEqual((o[1] as string), input2);
                called = true;
                return output;
            });
            var result = generatedResult.InterceptedInstance.MethodOverloadMethodTest(input1, input2);
            Assert.IsTrue(called);
            Assert.AreEqual(output, result);
        }

        [TestMethod]
        public void MethodOverloadMethodTest2()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInputClass();
            var input2 = RandString();
            var input3 = RandInt();
            var output = RandInt();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodOverloadMethodTest), new Type[]
            {
                typeof(InputClass),
                typeof(string),
                typeof(int),
            },
            (o) =>
            {
                Assert.AreEqual((o[0] as InputClass).Input, input1.Input);
                Assert.AreEqual((o[1] as string), input2);
                Assert.AreEqual(o[2], input3);
                called = true;
                return output;
            });
            var result = generatedResult.InterceptedInstance.MethodOverloadMethodTest(input1, input2, input3);
            Assert.IsTrue(called);
            Assert.AreEqual(output, result);
        }


        [TestMethod]
        public void MethodClassVoid()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var output = RandOutputClass();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodClassVoid), new Type[]
            {
            },
            (o) =>
            {
                called = true;
                return output;
            });
            var result = generatedResult.InterceptedInstance.MethodClassVoid();
            Assert.IsTrue(called);
            Assert.AreEqual(output.Output, result.Output);
        }

        [TestMethod]
        public void MethodClassSinglePrimitive()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInt();
            var output = RandOutputClass();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodClassSinglePrimitive), new Type[]
            {
                typeof(int),
            },
            (o) =>
            {
                Assert.AreEqual(o[0], input1);
                called = true;
                return output;
            });
            var result = generatedResult.InterceptedInstance.MethodClassSinglePrimitive(input1);
            Assert.IsTrue(called);
            Assert.AreEqual(output.Output, result.Output);
        }

        [TestMethod]
        public void MethodClassManyPrimitive()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInt();
            var input2 = RandInt();
            var input3 = RandFloat();
            var output = RandOutputClass();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodClassManyPrimitive), new Type[]
            {
                typeof(int),
                typeof(int),
                typeof(float),
            },
            (o) =>
            {
                Assert.AreEqual(o[0], input1);
                Assert.AreEqual(o[1], input2);
                Assert.AreEqual(o[2], input3);
                called = true;
                return output;
            });
            var result = generatedResult.InterceptedInstance.MethodClassManyPrimitive(input1, input2, input3);
            Assert.IsTrue(called);
            Assert.AreEqual(output.Output, result.Output);
        }

        [TestMethod]
        public void MethodClassSingleClass()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInputClass();
            var output = RandOutputClass();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodClassSingleClass), new Type[]
            {
                typeof(InputClass),
            },
            (o) =>
            {
                Assert.AreEqual((o[0] as InputClass).Input, input1.Input);
                called = true;
                return output;
            });
            var result = generatedResult.InterceptedInstance.MethodClassSingleClass(input1);
            Assert.IsTrue(called);
            Assert.AreEqual(output.Output, result.Output);
        }

        [TestMethod]
        public void MethodClassManyClass()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInputClass();
            var input2 = RandInputClass();
            var input3 = RandString();
            var output = RandOutputClass();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodClassManyClass), new Type[]
            {
                typeof(InputClass),
                typeof(InputClass),
                typeof(string),
            },
            (o) =>
            {
                Assert.AreEqual((o[0] as InputClass).Input, input1.Input);
                Assert.AreEqual((o[1] as InputClass).Input, input2.Input);
                Assert.AreEqual(o[2], input3);
                called = true;
                return output;
            });
            var result = generatedResult.InterceptedInstance.MethodClassManyClass(input1, input2, input3);
            Assert.IsTrue(called);
            Assert.AreEqual(output.Output, result.Output);
        }

        [TestMethod]
        public void MethodClassMixed()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInputClass();
            var input2 = RandInt();
            var input3 = RandInputClass();
            var output = RandOutputClass();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodClassMixed), new Type[]
            {
                typeof(InputClass),
                typeof(int),
                typeof(InputClass),
            },
            (o) =>
            {
                Assert.AreEqual((o[0] as InputClass).Input, input1.Input);
                Assert.AreEqual(o[1], input2);
                Assert.AreEqual((o[2] as InputClass).Input, input3.Input);
                called = true;
                return output;
            });
            var result = generatedResult.InterceptedInstance.MethodClassMixed(input1, input2, input3);
            Assert.IsTrue(called);
            Assert.AreEqual(output.Output, result.Output);
        }

        [TestMethod]
        public void MethodClassMixed2()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandInt();
            var input2 = RandInputClass();
            var input3 = RandInt();
            var output = RandOutputClass();
            generatedResult.Interceptor.SetMethodImplementation(nameof(ITestInterface.MethodClassMixed2), new Type[]
            {
                typeof(int),
                typeof(InputClass),
                typeof(int),
            },
            (o) =>
            {
                Assert.AreEqual(o[0], input1);
                Assert.AreEqual((o[1] as InputClass).Input, input2.Input);
                Assert.AreEqual(o[2], input3);
                called = true;
                return output;
            });
            var result = generatedResult.InterceptedInstance.MethodClassMixed2(input1, input2, input3);
            Assert.IsTrue(called);
            Assert.AreEqual(output.Output, result.Output);
        }

        #endregion RepetativeMethodTesting

        [TestMethod]
        public void SetImplementationParamterPrimitiveSet()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandFloat();
            generatedResult.Interceptor.SetSetPropertyImplementation(nameof(ITestInterface.PropertyPrimitiveReadWrite), (i) =>
            {
                called = true;
                Assert.AreEqual(input1, i);
            });

            generatedResult.InterceptedInstance.PropertyPrimitiveReadWrite = input1;

            Assert.IsTrue(called);
        }

        [TestMethod]
        public void SetImplementationParamterPrimitiveGet()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var output1 = RandInt();
            generatedResult.Interceptor.SetGetPropertyImplementation(nameof(ITestInterface.PropertyPrimitiveReadonly), () =>
            {
                called = true;
                return output1;
            });

            var result = generatedResult.InterceptedInstance.PropertyPrimitiveReadonly;

            Assert.AreEqual(output1, result);
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void SetImplementationParamterComplexSet()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var input1 = RandOutputClass();
            generatedResult.Interceptor.SetSetPropertyImplementation(nameof(ITestInterface.PropertyComplexReadWrite), (i) =>
            {
                called = true;
                Assert.AreEqual(input1, i);
            });

            generatedResult.InterceptedInstance.PropertyComplexReadWrite = input1;

            Assert.IsTrue(called);
        }

        [TestMethod]
        public void SetImplementationParamterComplexGet()
        {
            var generatedResult = TypeFactory.BuildType<ITestInterface>();

            var called = false;
            var output1 = RandOutputClass();
            generatedResult.Interceptor.SetGetPropertyImplementation(nameof(ITestInterface.PropertyComplexReadonly), () =>
            {
                called = true;
                return output1;
            });

            var result = generatedResult.InterceptedInstance.PropertyComplexReadonly;

            Assert.AreEqual(output1, result);
            Assert.IsTrue(called);
        }

        private int RandInt()
        {
            return new Random().Next();
        }

        private float RandFloat()
        {
            return (float)(new Random().NextDouble());
        }

        private InputClass RandInputClass()
        {
            return new InputClass(Guid.NewGuid().ToString());
        }

        private OutputClass RandOutputClass()
        {
            return new OutputClass(Guid.NewGuid().ToString());
        }

        private string RandString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
