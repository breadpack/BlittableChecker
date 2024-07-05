# BlittableChecker

BlittableChecker는 Unity 프로젝트에서 구조체가 블리터블(Blittable) 타입을 준수하는지 검증하는 패키지입니다.
이 패키지는 구조체에 `[Blittable]` 속성을 사용하여 컴파일 타임에 블리터블 타입을 검증합니다.
블리터블이 아닌 필드가 발견되면 컴파일 에러를 발생시킵니다.

## 주요 기능

- **구조체 검증**: 구조체가 블리터블 타입인지 확인하여 컴파일 타임 에러를 발생시킵니다.
- **상세 진단**: 블리터블이 아닌 필드와 그 이유를 제공합니다.

## 설치

Unity Package Manager에서 git URL을 사용하여 설치할 수 있습니다. 아래 URL을 사용하세요:

```
https://github.com/breadpack/BlittableChecker.git?path=UnityPackage
```

## 사용 방법

### 1. 구조체 정의 및 `[Blittable]` 속성 추가

``` csharp
using BlittableChecker;

[Blittable]
public struct MyStruct
{
public int MyInt;
public double MyDouble;
}
```

### 2. 컴파일 타임 검증

구조체가 블리터블이 아닌 필드를 포함할 경우, 컴파일 에러가 발생합니다:

``` csharp
[Blittable]
public struct RegularArrayStruct
{
    public int[] IntArray; // Regular arrays are not blittable.
}

[Blittable]
public struct ReferenceTypeStruct
{
    public string StringValue; // string is not blittable.
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
    public ReferenceTypeStruct   ReferenceType;      // Non-blittable struct field
    public RegularArrayStruct    RegularArrayStruct; // Non-blittable struct field
}

[Blittable]
public struct AutoPropertyStruct
{
    public int IntValue { get; set; } // Auto properties are not blittable.
}

[Blittable]
public struct NestedNonBlittableStruct
{
    public BasicTypesStruct    BasicTypes;
    public ReferenceTypeStruct ReferenceType; // Non-blittable struct field
}
```

## 예제

블리터블 구조체 예제:

``` csharp
[Blittable]
public struct ValidStruct
{
public int MyInt;
public double MyDouble;
public fixed char MyCharArray[256]; // Fixed arrays are blittable.
}
```

## 라이센스

이 프로젝트는 MIT 라이센스 하에 배포됩니다. 자세한 내용은 `LICENSE` 파일을 참고하세요.

## 문의

- **이름**: 빵봉투
- **이메일**: milennium9@gmail.com
- **GitHub**: [breadpack](https://github.com/breadpack)
