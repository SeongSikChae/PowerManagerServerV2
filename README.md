# PowerManagerServerV2

## history

### V1.0.0 - f62ed28d (Pre Release)

* 분 단위 전력량 추가
* 분, 시간, 일 단위 온도 추가
* 통계 Segment가 많아졌을 때 LevelDB에 Segment을 찾는 과정에서 과다한 CPU 사용이 발생됨에 따라 LevelDB를 제거 후 InMemory 자료 구조를 통한 통계 처리로 변경 
* 1분 단위 및 서버 종료 시 InMemory 통계 데이터를 Persistent File로 기록
* 서버 기동 시 Persistent 통계 데이터를 InMemory로 Load
* P.S 서버 강제 종료 시 마지막 기록한 통계 데이터 이후에 발생된 세그먼트는 유실

### V1.0.0 - 4ddf6ec1 (Pre Release)

* 서버 종료 시 스케줄에 설정된 On/Off 동작이 일괄로 실행되버리는 Bug Fix
* 우측 상단에 사용자 프로필 팝업 실행 시 클라이언트 인증서에 Email 정보가 누락되어 있을경우 Error 발생되는 Bug Fix
* Sqlite DB 및 Level DB 사용 시 비효율적인 Thread Locking 최적화
* 전력량 통계 차트에서 Min, Max 기준 계산 방법 변경

### V1.0.0 - d7c7adfa (Pre Release)

* SmartAdmin2 기반 웹화면 개발

## Config.yml
* ServerCertificate: 서버 인증서 파일 경로 (필수)
* ServerCertificatePassword: 서버 인증서 패스워드 (필수)
* DbPath: SQLite Database 경로 (필수)
* WebHttpPort: Http 접속 포트 (기본값: 80)
* WebHttpsPort: Https 접속 포트 (기본값: 443)
* ApiPort: 기기 API 인증 접속 포트 (기본값: 18443)
* MqttPort: MQTT 포트 (기본값: 1803)
* MqttsPort: 보안 MQTT 포트 (기본값: 8883)
* MqttServerBacklog: MQTT 접속 대기 큐 (기본값: 100)
* MqttKeepAlliveInterval: MQTT Comunication Timeout (기본값: 30000)(밀리초)
* HTTP2: HTTP2 프로토콜 여부 (기본값: false)
* IncludeCipherSuites: HTTPS 암호화 수준 Whitelist (옵션)
* TelegramToken: 텔레그램 봇 Token (옵션)


## AdminThumbprint 값 입력법

```
openssl x509 -in "클라이언트 인증서 CRT 파일 경로" -noout -fingerprint -sha1
```
값을 소문자로 :을 제외하고 입력

## 스케줄용 Cron Expression 입력 방법

```
// 초 분 시 일 월 요일 년
// 요일의 경우 원래는 0(일) ~ 6(토) 지만 PowerManagerServer에선 1(일) ~ 7(월) 형태로 사용된다
// Example
* * * * * ? // 매 초 마다 - XX:XX:01, XX:XX:02, XX:XX:03, ...
0 * * * * ? // 매 분 마다 - XX:00:00, XX:01:00, XX:02:00, ...
5 * * * * ? // 매분 5초에 - XX:00:05, XX:01:05, XX:02:05, ...
0 0 9 * * ? // 매일 9시에 - XXXX-XX-00 09:00:00, XXXX-XX-01 09:00:00, XXXX-XX-02 09:00:00, ...
0 0 0 1 * ? // 매월 1일에 - XXXX-01-01 00:00:00, XXXX-02-01 00:00:00, XXXX-03-01 00:00:00, ...
0 0 0 1 9 ? // 매년 9월 1일에 - 2023-09-01 00:00:00, 2024-09-01 00:00:00, 2025-09-01 00:00:00, ...
0 0 0 ? * 2-6 // 매주 월요일에서 금요일 0시에
0 0 0 1 1 ? 2023 // 2023년 1월 1일 0시 0분 0초
```

## IncludeCipherSuites 설정 방법

```
IncludeCipherSuites:
  - "TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384"
  - "TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256"
```
