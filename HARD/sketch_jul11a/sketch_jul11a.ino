const int ledPin = 13; // LEDが接続されているピンの番号を選択します

void setup() {
  pinMode(ledPin, OUTPUT); // LEDのピンを出力モードに設定
  Serial.begin(9600);      // シリアル通信の初期化
}

void loop() {
  if (Serial.available() > 0) { // データが利用可能な場合
    char data = Serial.read();  // データを読み込む
    if (data == '1') {          // データが "1" の場合
      digitalWrite(ledPin, HIGH); // LEDを点灯
    } else {                     // それ以外の場合
      digitalWrite(ledPin, LOW);  // LEDを消灯
    }
  }
}
