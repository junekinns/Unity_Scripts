# Avovovo  


## Preview  
![Preview](Avovovo.gif)
* Full Play video : https://www.youtube.com/watch?v=mzS26esYZOk  



## One Page Game Design Document
![Preview](AvovovoGDD.jpg)  

### Development Note 
* 6180 the moon 게임과 완전히 동일한 시스템  
* 이미지만 다름. 그외에는 완전히 동일. 
* 타일맵 강의 참고 : https://www.youtube.com/watch?v=dtpthaIYa8g&list=PL412Ym60h6uunDV_SaHfBXzfMhBjOFTnj  

* 사용 에셋: BayatGames, cinemachine, Free food Pack  
 
* 04/09/월   
 -1탄(튜토리얼) 타일 맵(받아놓은 에셋사용)  
 -Player : 이동, 점프(but 타일맵 Ground감지가 안되고있음)   
 -Goal : 회전, 그냥 껍데기만 있어야 함  

* 04/10/화  
 -Player 바닥감지해서 점프횟수 1회로 제한, //완료  
 -GoalPosition과 Player 자석 효과 구현(모든방향에서) // 50% 완료  
 -GoalIn을 부드럽게 구현하고 싶어서 별짓을 다 해봤지만 안됨, 코루틴을 써야하나?  
 -Goal까지 천천히 갔다가 골에 들어가면 안움직여야되는데 계속 흔들림  
  
* 04/11수  
 -GoalIn 부드럽게 구현 50% //완료  
 -Player 위아래 통과할 수 있도록 //완료  
 -맵, 스테이지 넘어가는 거 구현 //완료  
 -맵밑부분 튕기기 구현//완료  
 -플레이어(씨앗,아보카도) 이미지 구현 //완료  
 -기본적인 UI구현 // 50% 완료  
 -점프, 적 사운드 구현. SoundManager스크립트 추가(모든사운드는 여기서 관리)  
 -TurnChange 스크립트 모두 삭제(트리거로 스테이지 전환하는거 말고 다른방법 필요)  
 -스테이지 전환 모두 Player스크립트에서 해결하도록  
 -스테이지 전환 SceneManager.LoadScene("Stage01"); 말고 씬 인덱스 부여해서 하는방법으로 해보자(한 스크립트에서 해결 가능)
 ->네이버 골드메탈 블로그    

* 04/14 토  
 -시네머신 삭제하고 카메라 Lerp방식으로 바꿈.  
 -타일맵 이상하던거 수정  
 -캐릭터 속도, 점프력, 중력 수정  
 
* 04/15 일  
 -Bounce할때 점프 안되도록 수정  
 -게임 시작 화면 먼저  
 -UI는 에셋 이미지로 해결  
 -GoalIn할때 딜레이, 이펙트  
 -스테이지 넘어갈 때 Trigger말고 다른방법 검색 //코루틴으로 해결  
 -좌/우 범위 빠져나가면 어떻게 할 것인지? //Wall로 해결..(임시방편)  
  
* 05/10 목  
 -BGM 저작권 문제 없는걸로 수정(5개 배열로 저장해서 랜덤하게 재생하도록 할 지?)  
 -시작화면 배경 무한스크롤로 변경(Meshrender 컴포넌트에서 Offset 조정하는 것으로)  
 -Joystick이랑 JumpButton 넣어봤는데 별로 안어울려서 다른 방법 생각해야 함  
 -씬 전환 트랜지션(Fade in / out)  
 
* 05/11 금  
 -Die, Goal In 카메라 Zoom In 추가(Math.f Lerp)  

* 05/25 금  
 -Add animation (Die) -> how to keep rotation?  
 -Cinematic camera  

* 06/07 목  
 -모바일 컨트롤러 매 씬마다 캔ㄴ버스랑 같이 직접 만들어야 함. 자동으로 불러오는거 실패..  
 -다~ 되면 맥북으로 옮겨서 Iphone Build  
 -맵 레벨디자인 수정 -> 몇 탄 까지?  
 -엔딩 시네마틱.....할 수 있을까?ㄴㅇㅁㄹㄴㅇㄹㄷ  


 ####################  BUG REPORT ###################  
 -GoalIn 손봐야함(코루틴 잘못 쓴듯) -> 해결  
 -떨어질때 어느정도 빨라지면 땅에 박혀버림  -> Rigidbody 컴포넌트에서 Collision Detect를 Continuous로 바꿧더니 해결?  
 -바닥도 그라운드 체크가 되어버려서 점프가 가능해짐. Bounce에 그라운드 0으로 설정하는 코드 추가. ->해결  
 -옆/모서리에 쿵 찌면 관성처럼 계속 그 방향으로 가려고 함 -> Rigidbody Constraints freeze로 해결...  
 -옆(벽면)도 그라운드 체크가 되어버려서 점프가 가능해져서 실수로 점프를 눌러버리면, 점프하고싶을때 못해버림.  
 -일부 바닥면에서 그라운드 체크 되어버려서 점프가 가능한 버그(Bounce 조정해야 함)  
 -움직일 때 버버벅 거리는거 왜그렇지? Transform으로 이동하는게 문제인듯...(보류)   
 -애니메이션 추가  
 -모바일 Stage text'만' DontDestroyOnLoad // 완료   
