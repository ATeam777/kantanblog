const week = ["日", "月", "火", "水", "木", "金", "土"];
const today = new Date();
// 月末だとずれる可能性があるため、1日固定で取得
var showDate = new Date(today.getFullYear(), today.getMonth(), 1);

// 初期表示
window.onload = function () {
    showProcess(today, calendar);
};
// 前の月表示
function prev(){
    showDate.setMonth(showDate.getMonth() - 1);
    showProcess(showDate);
}

// 次の月表示
function next(){
    showDate.setMonth(showDate.getMonth() + 1);
    showProcess(showDate);
}

// カレンダー表示
function showProcess(date) {
    var year = date.getFullYear();
    var month = date.getMonth();
    document.querySelector('#header').innerHTML = year + "年 " + (month + 1) + "月";

    var calendar = createProcess(year, month);
    document.querySelector('#calendar').innerHTML = calendar;
}

// カレンダー作成
function createProcess(year, month) {
    // 曜日
    var calendar = "<table class='tableborder'><tr class='dayOfWeek'>";
    for (var i = 0; i < week.length; i++) {
        calendar += "<th class='thtdborder'>" + week[i] + "</th>";
    }
    calendar += "</tr>";

    var count = 0;
    var startDayOfWeek = new Date(year, month, 1).getDay();
    var endDate = new Date(year, month + 1, 0).getDate();
    var lastMonthEndDate = new Date(year, month, 0).getDate();
    var row = Math.ceil((startDayOfWeek + endDate) / week.length);

    let url = "https://www.jma.go.jp/bosai/forecast/data/forecast/400000.json";

    fetch(url)
        .then(function (response) {
            return response.json();
        })
        .then(function (weather) {
            console.log(weather);
            // 特定の地域(今回は福岡)だけ選択して変数に詰め直す
            let area = weather[0].timeSeries[0].areas[0];
            console.log(area);
            // 発表者と報告日時の情報を画面に書き出す
            document.getElementById("publishingOffice").lastElementChild.textContent = weather[0].publishingOffice;
            document.getElementById("reportDatetime").lastElementChild.textContent = weather[0].reportDatetime;
            // 特定地域の情報を画面に書き出す
            document.getElementById("targetArea").lastElementChild.textContent = area.area.name;
            document.getElementById("today").lastElementChild.textContent = area.weathers[0];
            document.getElementById("tomorrow").lastElementChild.textContent = area.weathers[1];
            document.getElementById("dayAfterTomorrow").lastElementChild.textContent = area.weathers[2];
        });

    // 1行ずつ設定
    for (var i = 0; i < row; i++) {
        calendar += "<tr>";
        // 1colum単位で設定

        for (var j = 0; j < week.length; j++) {

            if (i == 0 && j < startDayOfWeek) {
                // 1行目で1日まで先月の日付を設定
                calendar += "<td class='disabled thtdborder'>" + (lastMonthEndDate - startDayOfWeek + j + 1) + "</td>";
            } else if (count >= endDate) {
                // 最終行で最終日以降、翌月の日付を設定
                count++;
                calendar += "<td class='disabled thtdborder'>" + (count - endDate) + "</td>";
            } else {
                // 当月の日付を曜日に照らし合わせて設定
                count++;
                /*calendar += "<a asp-page='ArticleView' asp-route-date='@item => item.Day ==count'>";*/
                if(year == today.getFullYear()
                  && month == (today.getMonth())
                  && count == today.getDate()){
                    calendar += "<td class='today thtdborder'>" + count + "</td>";
                } else {
                    calendar += "<td class='thtdborder'>" + count + "</td>";
                }
                /*calendar += "</a>":*/
                calendar += "</td>";

            }
        }
        calendar += "</tr>";
    }
    return calendar;
}