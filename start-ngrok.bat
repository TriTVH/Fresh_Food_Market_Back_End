@echo off
echo ==========================================
echo       DUNG NGROK CHO FRESH MARKET
echo ==========================================
echo 1. Hay dam bao ban da chay: npx ngrok config add-authtoken <TOKEN>
echo 2. Dang mo cong 5000 cho Ocelot Gateway...
echo.
npx ngrok http 5000 --host-header="localhost:5000"
pause
