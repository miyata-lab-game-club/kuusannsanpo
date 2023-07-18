#define M5STACK_MPU6886 
#include <M5Stack.h>
 
float accX = 0.0F;
float accY = 0.0F;
float accZ = 0.0F;
 
float gyroX = 0.0F;
float gyroY = 0.0F;
float gyroZ = 0.0F;
 
float pitch = 0.0F;
float roll  = 0.0F;
float yaw   = 0.0F;
 
int mode = -1;
 
void setup() {
  // 初期設定
  M5.begin();
  M5.Power.begin();
  M5.IMU.Init();

    // 起動画面設定
  M5.Lcd.fillScreen(TFT_NAVY);
  M5.Lcd.setTextColor(TFT_WHITE);
  M5.Lcd.setTextSize(2);
  M5.Lcd.printf("START");
  Serial.begin(9600);  
}
 
void loop() {
  M5.update();
 
  // センサデータ取得
  M5.IMU.getGyroData(&gyroX, &gyroY, &gyroZ);
  M5.IMU.getAccelData(&accX, &accY, &accZ);
  M5.IMU.getAhrsData(&pitch, &roll, &yaw);
  Serial.println(gyroX);
  // ボタンA押下イベント
  if ( mode == -1 || M5.BtnA.wasReleased() ) {
    mode++;
    mode = mode % 3;
 
    // ボタンAを押す毎に、ジャイロ→加速度→姿勢角度を順番に切り替える
    // プロッタ用のタイトル出力
    if ( mode == 0 ) {
      Serial.printf("gyroX,gyroY,gyroZ\n");
    } else if ( mode == 1 ) {
      Serial.printf("accX,accY,accZ\n");
    } else if ( mode == 2 ) {
      Serial.printf("pitch,roll,yaw\n");
    }
  }
 
  // データ出力
  if ( mode == 0 ) {
    Serial.printf("%6.2f,%6.2f,%6.2f\n", gyroX, gyroY, gyroZ);
    
    M5.Lcd.fillScreen(TFT_OLIVE);
    M5.Lcd.setCursor(0, 100);
    M5.Lcd.println("Gyro");
    M5.Lcd.printf("gyro=(%5.1f, %5.1f, %5.1f)", gyroX, gyroY, gyroZ);

  } else if ( mode == 1 ) {
    Serial.printf("%5.2f,%5.2f,%5.2f\n", accX, accY, accZ);
    
    M5.Lcd.fillScreen(TFT_GREEN);
    M5.Lcd.setCursor(0, 100);
    M5.Lcd.println("Acc");
    M5.Lcd.printf("acc=(%5.1f, %5.1f, %5.1f)", accX, accY, accZ);

  } else if ( mode == 2 ) {
    Serial.printf("%5.2f,%5.2f,%5.2f\n", pitch, roll, yaw);

    M5.Lcd.fillScreen(TFT_PINK);
    M5.Lcd.setCursor(0, 100);
    M5.Lcd.println("Ahrs");
    M5.Lcd.printf("PRY=(%5.1f, %5.1f, %5.1f)", pitch, roll, yaw);

  }
 
  delay(200);
}