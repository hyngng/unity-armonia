## **2.5D 횡스크롤 비주얼 게임**

> 자세한 사항은 [블로그](https://hyngng.github.io//posts/armonia-developing-cancelled/)에서 확인할 수 있습니다!

## **무엇을 만들었나**

```mermaid
classDiagram
    class MainManager {
        + State: Phase
        + SelectedObject: GameObject
        + ActivatedObject: GameObject
    }
    class ObjectGenerator {
        + Livings: List~GameObject~
        + NonLivings: List~GameObject~
        + Population: Dictionary~string,int~
    }
    class Living {
        + IsSelected: bool
        + IsActivated: bool
        + Speed: float
    }
    class NonLiving {
        + InteractionDistance: float
        + ObjectAttractCycle: float
        + Feature: List~Sprite~
    }

    Living <|-- People
    Living <|-- Pigeon
    NonLiving <|-- VendingMachine
    NonLiving <|-- Bench
```

[Living](https://github.com/hyngng/unity-armonia/blob/master/Assets/Scripts/Living/Living.cs) 부모 클래스를 만들고 [People](https://github.com/hyngng/unity-armonia/blob/master/Assets/Scripts/Living/People/People.cs)이나 [Pigeon](https://github.com/hyngng/unity-armonia/blob/master/Assets/Scripts/Living/Pigeon/Pigeon.cs) 등의 하위 클래스에서 참조하도록 만들었습니다.
