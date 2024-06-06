window.onload = function () {

    let url = "https://www.jma.go.jp/bosai/forecast/data/forecast/400000.json";

    fetch(url)
        .then(function (response) {
            return response.json();
        })
        .then(function (weather) {

            //console.log(weather);

            // 特定の地域(今回は福岡)だけ選択して変数に詰め直す
            let area = weather[0].timeSeries[0].areas[0];
            // 発表者と報告日時の情報を画面に書き出す
            document.getElementById("publishingOffice").lastElementChild.textContent = weather[0].publishingOffice;
            //document.getElementById("reportDatetime").lastElementChild.textContent = weather[0].reportDatetime;
            // 特定地域の情報を画面に書き出す
            document.getElementById("targetArea").lastElementChild.textContent = area.area.name;
            document.getElementById("today").lastElementChild.textContent = area.weathers[0];
            document.getElementById("tomorrow").lastElementChild.textContent = area.weathers[1];
            document.getElementById("dayAfterTomorrow").lastElementChild.textContent = area.weathers[2];
        });

    // 現在の年月の取得
    var current = new Date();
    var year = current.getFullYear();
    var month = current.getMonth() + 1;

    // カレンダーの表示
    var wrapper = document.getElementById('calendar');
    add_calendar(wrapper, year, month);
}

/**
 * 指定した年月のカレンダーを表示する
 * @param {object} wrapper - カレンダーを追加する親要素
 * @param {number} year    - 年の指定
 * @param {number} month   - 月の指定
 */
function add_calendar(wrapper, year, month) {

    //Jsonにして渡す
    var data = JSON.parse(document.getElementById('listDate').value);

    //DEBUG
    //console.log(data);
    // 現在カレンダーが追加されている場合は一旦削除する
    wrapper.textContent = null;

    // カレンダーに表示する内容を取得
    var headData = generate_calendar_header(wrapper, year, month);
    var bodyData = generate_month_calendar(year, month, data);

    // カレンダーの要素を追加
    wrapper.appendChild(headData);
    wrapper.appendChild(bodyData);
}

/**
 * 指定した年月のカレンダーのヘッダー要素を生成して返す
 * @param {object} wrapper - カレンダーを追加する親要素
 * @param {number} year    - 年の指定
 * @param {number} month   - 月の指定
 */
function generate_calendar_header(wrapper, year, month) {

    // 前月と翌月を取得
    var nextMonth = new Date(year, (month - 1));
    nextMonth.setMonth(nextMonth.getMonth() + 1);
    var prevMonth = new Date(year, (month - 1));
    prevMonth.setMonth(prevMonth.getMonth() - 1);

    // ヘッダー要素
    var cHeader = document.createElement('div');
    cHeader.className = 'calendar-header';


    // 見出しの追加
    var cTitle = document.createElement('div');
    cTitle.className = 'calendar-header__title';
    var cTitleText = document.createTextNode(year + '年' + month + '月');
    cTitle.appendChild(cTitleText);
    cHeader.appendChild(cTitle);

    // 前月ボタンの追加
    var cPrev = document.createElement('button');
    cPrev.className = 'calendar-header-button';
    cPrev.id = 'prev';
    
    var cPrevText = document.createTextNode('<');
    cPrev.appendChild(cPrevText);

    // 前月ボタンをクリックした時のイベント設定
    cPrev.addEventListener('click', function () {
        add_calendar(wrapper, prevMonth.getFullYear(), (prevMonth.getMonth() + 1));
    }, false);
    cHeader.appendChild(cPrev);

    // 翌月ボタンの追加
    var cNext = document.createElement('button');
    cNext.className = 'calendar-header-button';
    cNext.id = 'next';
    var cNextText = document.createTextNode('>');
    cNext.appendChild(cNextText);

    // 翌月ボタンをクリックした時のイベント設定
    cNext.addEventListener('click', function () {
        add_calendar(wrapper, nextMonth.getFullYear(), (nextMonth.getMonth() + 1));
    }, false);
    cHeader.appendChild(cNext);

    return cHeader;
}

/**
 * 指定した年月のカレンダー要素を生成して返す
 * @param {number} year     - 年の指定
 * @param {number} month    - 月の指定
 * @param {object} linkData - リンクを設定する日付の情報
 */
function generate_month_calendar(year, month, linkData) {
    var weekdayData = ['日', '月', '火', '水', '木', '金', '土'];
    // カレンダーの情報を取得
    var calendarData = get_month_calendar(year, month);

    var i = calendarData[0]['weekday']; // 初日の曜日を取得
    // カレンダー上の初日より前を埋める
    while (i > 0) {
        i--;
        calendarData.unshift({
            day: '',
            weekday: i
        });
    }
    var i = calendarData[calendarData.length - 1]['weekday']; // 末日の曜日を取得
    // カレンダー上の末日より後を埋める
    while (i < 6) {
        i++;
        calendarData.push({
            day: '',
            weekday: i
        });
    }

    // カレンダーの要素を生成
    var cTable = document.createElement('table');
    cTable.className = 'tableborder';

    var insertData = '';
    // 曜日部分の生成
    insertData += '<thead>';
    insertData += '<tr  class=\'dayOfWeek\'>';
    for (var i = 0; i < weekdayData.length; i++) {
        insertData += '<th class=\'thtdborder\'>';
        insertData += weekdayData[i];
        insertData += '</th>';
    }
    insertData += '</tr>';
    insertData += '</thead>';

    // 日付部分の生成
    insertData += '<tbody>';
    for (var i = 0; i < calendarData.length; i++) {
        if (calendarData[i]['weekday'] <= 0) {
            insertData += '<tr>';
        }

        var ymd = year + '-' + month + '-' + calendarData[i]['day'];

        //今日の日付を取得
        var now = new Date();
        var formattedDate = now.toLocaleDateString('ja-JP');
        var repDate = formattedDate.replaceAll('/','-');
        
        if (ymd == repDate) {
            insertData += '<td class=\'tableborder today\'>';
        } else {
            insertData += '<td class=\'thtdborder\'>';
        }

        //リンク処理
        for (var j = 0; j < linkData.length; j++) {
            if (linkData[j]['date'] === ymd) {
                insertData += '<a href="' + linkData[j]['link'] + '">' + calendarData[i]['day'] + '</a>';
                break;
            }
            if (j >= linkData.length - 1) {
                insertData += calendarData[i]['day'];
            }
        }
        // ----------------------
        insertData += '</td>';

        if (calendarData[i]['weekday'] >= 6) {
            insertData += '</tr>';
        }
    }
    insertData += '</tbody>';

    cTable.innerHTML = insertData;
    return cTable;
}

/**
 * 指定した年月のカレンダー情報を返す
 * @param {number} year  - 年の指定
 * @param {number} month - 月の指定
 */
function get_month_calendar(year, month) {
    var firstDate = new Date(year, (month - 1), 1); // 指定した年月の初日の情報
    var lastDay = new Date(year, (firstDate.getMonth() + 1), 0).getDate(); // 指定した年月の末日
    var weekday = firstDate.getDay(); // 指定した年月の初日の曜日

    var calendarData = []; // カレンダーの情報を格納
    var weekdayCount = weekday; // 曜日のカウント用
    for (var i = 0; i < lastDay; i++) {
        calendarData[i] = {
            day: i + 1,
            weekday: weekdayCount
        }
        // 曜日のカウントが6(土曜日)まできたら0(日曜日)に戻す
        if (weekdayCount >= 6) {
            weekdayCount = 0;
        } else {
            weekdayCount++;
        }
    }
    return calendarData;
}


//星の表現
window.addEventListener("DOMContentLoaded", () => {
    // 星を表示するための親要素を取得
    const stars = document.querySelector(".stars");

    // 星を生成する関数
    const createStar = () => {
        const starEl = document.createElement("span");
        starEl.className = "star";
        const minSize = 2; // 星の最小サイズを指定
        const maxSize = 6; // 星の最大サイズを指定
        const size = Math.random() * (maxSize - minSize) + minSize;
        starEl.style.width = `${size}px`;
        starEl.style.height = `${size}px`;
        starEl.style.left = `${Math.random() * 100}%`;
        starEl.style.top = `${Math.random() * 100}%`;
        starEl.style.animationDelay = `${Math.random() * 10}s`;
        stars.appendChild(starEl);
    };

    // for文で星を生成する関数を指定した回数呼び出す
    for (let i = 0; i <= 400; i++) {
        createStar();
    }
});


//書き込み風Title表示:野中さん作成
function autoType(elementClass, typingSpeed) {
    var thhis = $(elementClass);
    thhis.css({
        "position": "relative",
        "display": "inline-block"
    });
    thhis.prepend('<div class="cursor" style="right: initial; left:0;"></div>');
    thhis = thhis.find(".text-js");
    var text = thhis.text().trim().split('');
    var amntOfChars = text.length;
    var newString = "";
    thhis.text("|");
    setTimeout(function () {
        thhis.css("opacity", 1);
        thhis.prev().removeAttr("style");
        thhis.text("");
        for (var i = 0; i < amntOfChars; i++) {
            (function (i, char) {
                setTimeout(function () {
                    newString += char;
                    thhis.text(newString);
                }, i * typingSpeed);
            })(i + 1, text[i]);
        }
    }, 1500);
}

$(document).ready(function () {
    // Now to start autoTyping just call the autoType function with the 
    // class of outer div
    // The second paramter is the speed between each letter is typed.   
    autoType(".type-js", 200);
});


//コメントを三点リーダーで省略
document.addEventListener("DOMContentLoaded", function () {
    function truncateText(element, maxLength) {
        let truncated = element.innerText;

        if (truncated.length > maxLength) {
            truncated = truncated.substr(0, maxLength) + '...';
        }
        element.innerText = truncated;
    }

    const elements = document.querySelectorAll('.balloon');
    elements.forEach(function (element) {
        truncateText(element, 30); // 最大100文字まで表示
    });
});


//投稿ボタンの動き
$(function () {
    $(".button").mousedown(function () {
        var fancyButton = $(this).closest('.fancy-button');
        fancyButton.bind('animationend webkitAnimationEnd MSAnimationEnd oAnimationEnd', function () {
            fancyButton.removeClass('active');
        })
        fancyButton.addClass("active");
    });
});