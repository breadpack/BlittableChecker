using BlittableChecker;

public enum EnumIsBlittable {
    A,
    B,
    C
}

[Blittable]
public struct EnumStruct
{
    public EnumIsBlittable EnumValue;
}

[Blittable]
public struct BasicTypesStruct
{
    public int    IntValue;
    public double DoubleValue;
    public float  FloatValue;
}

[Blittable]
public struct PointerStruct
{
    public unsafe int* PointerValue; // 포인터는 블리터블이 아닙니다.
}

[Blittable]
public struct CircularReferenceStructA
{
    public CircularReferenceStructB StructB;
}

[Blittable]
public struct CircularReferenceStructB
{
    public CircularReferenceStructA StructA;
}

[Blittable]
public unsafe struct FixedArrayStruct
{
    public fixed byte ByteArray[16];
    public fixed char CharArray[32];
}

[Blittable]
public struct NestedBlittableStruct
{
    public BasicTypesStruct BasicTypes;
    public FixedArrayStruct FixedArrays;
}

[Blittable]
public struct RegularArrayStruct
{
    public int[] IntArray; // Regular arrays are not blittable.
}

[Blittable]
public struct ReferenceTypeStruct
{
    public string StringValue; // 이 필드는 블리터블이 아닙니다.
}

[Blittable]
public struct BoolArrayStruct
{
    public bool[] BoolArray; // Regular arrays of blittable types are not blittable.
}

[Blittable]
public struct MixedArrayStruct
{
    public        int[]            IntArray; // Regular arrays are not blittable.
    public        BasicTypesStruct BasicStruct;
    public unsafe FixedArrayStruct FixedArrayStruct;
}

[Blittable]
public struct ComplexNestedStruct
{
    public NestedBlittableStruct NestedBlittable;
    public ReferenceTypeStruct   ReferenceType;      // This field is not blittable.
    public RegularArrayStruct    RegularArrayStruct; // This field is not blittable.
}

[Blittable]
public struct AutoPropertyStruct
{
    public int IntValue { get; set; } // 자동 속성은 블리터블이 아닙니다.
}

[Blittable]
public struct NestedNonBlittableStruct
{
    public BasicTypesStruct    BasicTypes;
    public ReferenceTypeStruct ReferenceType; // 이 필드는 블리터블이 아닙니다.
}


internal class Program {
    public static void Main(string[] args) {
        Console.WriteLine("Hello, World!");
    }
}