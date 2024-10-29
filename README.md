# **2.5D 횡스크롤 형식의 비주얼 구경하는 게임**

![gameplay](https://cdn.jsdelivr.net/gh/hyngng/hyngng.github.io.resources@master/2024-10-23-armonia-developing-cancelled/gameplay.webp)

# **주요 특징**

- **개념적 설명**
    - 단순히 플레이어가 아무 조작을 하지 않아도 씬 내의 오브젝트들이 알아서 서로 상호작용하는 환경을 제공하여, 점수나 게임 종료 조건 없이 단순히 구경하며 돌아다니기만 하는 게임임.
- **개발 관련해서 시도한 것들**
    - get set 프로퍼티
    - 델리게이트
    - 싱글톤 패턴
    - 절차적 애니메이션
    - 이벤트 주도적 프로그래밍
    - PlasticSCM을 사용한 버전 관리
- **아트 관련해서 시도한 것들**
    - 직접 제작한 이미지와 애니메이션 애셋
    - 컷 애니메이션과 애니메이터 컴포넌트를 혼합
 
# **주요 코드**

- [MainManager.cs](https://github.com/hyngng/unity-armonia/blob/master/Assets/Scripts/System/MainManager.cs)
- [MapGenerator.cs](https://github.com/hyngng/unity-armonia/blob/master/Assets/Scripts/System/MapGenerator.cs)
- [ObjectGenerator.cs](https://github.com/hyngng/unity-armonia/blob/master/Assets/Scripts/System/ObjectGenerator.cs)
- [Living.cs](https://github.com/hyngng/unity-armonia/blob/master/Assets/Scripts/Living/Living.cs)
    - [People.cs](https://github.com/hyngng/unity-armonia/blob/master/Assets/Scripts/Living/People/People.cs)
    - [Pigeon.cs](https://github.com/hyngng/unity-armonia/blob/master/Assets/Scripts/Living/Pigeon/Pigeon.cs)
- [NonLiving.cs](https://github.com/hyngng/unity-armonia/blob/master/Assets/Scripts/Non-Living/NonLiving.cs)
    - [UtilityPole.cs](https://github.com/hyngng/unity-armonia/blob/master/Assets/Scripts/Non-Living/UtilityPole.cs)
    - [BendingMachine.cs](https://github.com/hyngng/unity-armonia/blob/master/Assets/Scripts/Non-Living/VendingMachine.cs)

<br>

# **후일담**
- [](https://hyngng.github.io//posts/armonia-developing-cancelled/)에서 더 자세히 확인할 수 있습니다!**
