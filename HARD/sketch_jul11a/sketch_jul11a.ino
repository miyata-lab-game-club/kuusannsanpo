const int ledPin = 13;    // LEDが接続されているピンの番号

void setup() {
  pinMode(ledPin, OUTPUT);    // LEDのピンを出力モードに設定
  Serial.begin(9600);         // シリアル通信の初期化
}

void loop() {
  // ボタンの状態を読み取り、シリアル通信で送信
 for (int i = 1; i <= 3; i++) {
    Serial.println(i);
    delay(1000);
  }
  if (Serial.available() > 0) { // データが利用可能な場合
    char data = Serial.read();  // データを読み込む
    if (data == '1') {          // データが "1" の場合
      digitalWrite(ledPin, HIGH); // LEDを点灯
    } else {                     // それ以外の場合
      digitalWrite(ledPin, LOW);  // LEDを消灯
    }
  }
}
