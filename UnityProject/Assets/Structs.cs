using BlittableChecker;

[Blittable]
public struct StringStruct {
    public string StringValue; // 이 필드는 블리터블이 아닙니다.
}
