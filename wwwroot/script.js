document.addEventListener("DOMContentLoaded", () => {
    const tg = window.Telegram.WebApp;
    tg.expand(); // Разворачиваем миниапп

    // Получаем имя пользователя
    const userName = tg.initDataUnsafe?.user?.first_name || "Гость";
    document.getElementById("user-name").textContent = userName;

    // Загружаем баланс с бэкенда (если подключен)
    const API_URL = "https://miniapp-backend.onrender.com"; // Если бекенд не нужен - закомментируй
    fetch(`${API_URL}/api/balance`)
        .then(response => response.json())
        .then(data => {
            document.getElementById("balance").textContent = data.balance;
        })
        .catch(error => console.error("Ошибка API:", error));

    function generateBarcode() {
        const barcodeData = Math.floor(Math.random() * 999999999999);
        JsBarcode("#barcode", barcodeData.toString(), {
            format: "CODE128",
            displayValue: false, // Убираем цифры под штрих-кодом
            fontSize: 0
        });
    }

    generateBarcode(); // Генерируем баркод

    // Кнопка закрытия миниаппа
    document.getElementById("close-app").addEventListener("click", () => {
        tg.close();
    });
});
