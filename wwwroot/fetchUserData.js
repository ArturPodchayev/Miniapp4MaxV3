async function getUserData() {
    try {
        console.log("🔵 Отправка запроса в API...");

        const requestBody = {
            apiLogin: "a0935f72d57747f2b4050936b866af09",
            phone: "+998332531966",
            type: "phone",
            organizationId: "e7db68f3-1068-4d79-96b3-fb20748d4623"
        };

        console.log("📤 Тело запроса:", requestBody);

        const response = await fetch('/api/Api/getCustomerInfo', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(requestBody)
        });

        console.log("📥 Ответ получен, статус:", response.status);

        if (!response.ok) {
            const errorText = await response.text();
            console.error("❌ Ошибка сервера:", errorText);
            throw new Error(`Ошибка сервера: ${response.status} - ${errorText}`);
        }

        const data = await response.json();
        console.log("✅ Данные от API:", data);

        if (!data || Object.keys(data).length === 0) {
            throw new Error("⚠️ Ответ API пустой!");
        }

        // Обновляем интерфейс
        document.getElementById('userName').textContent = data.name || "Гость";
        document.getElementById('balance').textContent = data.walletBalances?.[0]?.balance ?? "0";
        document.getElementById('cardNumber').textContent = data.cards?.[0]?.number ?? "Карта не найдена";
        document.getElementById('cardExpiry').textContent = data.cards?.[0]?.expiryDate ?? "Дата неизвестна";
        document.getElementById('phoneNumber').textContent = data.phone;

        // 🔥 Генерируем баркод по номеру карты или телефону, если карты нет
        const barcodeData = data.cards?.[0]?.number ?? data.phone;
        JsBarcode("#barcode", barcodeData, {
            format: "CODE128",
            displayValue: true,
            fontSize: 16,
            lineColor: "#fff",
            background: "#000",
            height: 50
        });

        console.log("🎉 Обновление интерфейса завершено!");

    } catch (err) {
        console.error("🚨 Ошибка в getUserData():", err);
        document.getElementById('userName').textContent = "Ошибка загрузки";
        document.getElementById('balance').textContent = "Ошибка загрузки";
        document.getElementById('cardNumber').textContent = "Ошибка загрузки";
        document.getElementById('phoneNumber').textContent = "Ошибка загрузки";
    }
}

document.addEventListener('DOMContentLoaded', () => {
    console.log("🟢 DOM загружен - запускаем getUserData()");
    getUserData();
});
