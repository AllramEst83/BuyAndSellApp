
var VISA_ALLA = "visa alla";
var $rows = $('#table tbody tr');
//-----------------------------------
///Sök------------------------------
$('#search').keyup(function () {
    var dropValue = $("#dropdown").each(function () { this.selectedIndex = 0; });
    var val = $.trim($(this).val()).replace(/ +/g, ' ').toLowerCase();
    if (val != "") {
        $rows.show().filter(function () {
            var text = $(this).children(":eq(" + "1" + ")").text().replace(/\s+/g, ' ').toLowerCase();
            //var text = $($(this).find('td')[1]).text().replace(/\s+/g, ' ').toLowerCase();
            //tabort för att inte söka på titel --> .children(":eq(" + "1" + ")")
            return !~text.indexOf(val);
        }).hide();
    }
});
//DropDownFilter-------------------------------------------------------
$(function() {
    $("#dropdown").change(function () {
        var selected = $(this).find("option:selected").text().toLowerCase();
        if (selected != VISA_ALLA) {
            $rows.show().filter(function () {
                var text = $(this).text().replace(/\s+/g, ' ').toLowerCase();
                return !~text.indexOf(selected);
            }).hide();        

        } else {
            $rows.show();
        }
    });
});
//documentReady-----------------
$(document).ready(function () {
    Sort();
    caputreUrl();
});
//Sort-------------------------
function Sort() {
   $('th').each(function (col) {
        $(this).hover(
            function () {
                $(this).addClass('focus');
            },
            function () {
                $(this).removeClass('focus');
            }
        );

        $(this).click(function () {
            if ($(this).is('.asc')) {
                $(this).removeClass('asc');
                $(this).addClass('desc selected');
                sortOrder = -1;
            } else {
                $(this).addClass('asc selected');
                $(this).removeClass('desc');
                sortOrder = 1;
            }
            $(this).siblings().removeClass('asc selected');
            $(this).siblings().removeClass('desc selected');
            var arrData = $('#table').find('tbody >tr:has(td)').get();

            arrData.sort(function (a, b) {
                var val1 = $(a).children('td').eq(col).text().toUpperCase();
                var val2 = $(b).children('td').eq(col).text().toUpperCase();
                if ($.isNumeric(val1) && $.isNumeric(val2))
                    return sortOrder === 1 ? val1 - val2 : val2 - val1;
                else
                    return (val1 < val2) ? -sortOrder : (val1 > val2) ? sortOrder : 0;
            });
            $.each(arrData, function (index, row) {
                $('tbody').append(row);
            });
        });
    });
}
//ImageUrlCapture---------------------------------------
function caputreUrl() {
    $(".testImage").attr("src", $("#imageInput").val());
}

//-----------------------------------------------------------------------------------------


//Code graveYard-------------------------------------------
//-------------------------------------------------------
//$("#Categori").change(function (evt) {
//    var categori = $(this).val();

//    $rows.show().filter(function () {
//        var text = $(this).text().replace(/\s+/g, ' ').toLowerCase();
//        return !~text.indexOf(categori);
//    }).hide();
//});



//<input type="text" id="search" placeholder="Type to search">
//    <div>@Html.DropDownList("Categories", null, new { @id = "Categori" })</div>



//Dropdown
//$rows.show().filter(function () {
//    var text = $(this).text().replace(/\s+/g, ' ').toLowerCase();
//    return !~text.indexOf(selected);
//}).hide();

////search
//$rows.show().filter(function () {
//    var text = $(this).children(":eq(" + "1" + ")").text().replace(/\s+/g, ' ').toLowerCase();
//    //var text = $($(this).find('td')[1]).text().replace(/\s+/g, ' ').toLowerCase()
//    ;//tabort för att inte söka på titel --> .children(":eq(" + "1" + ")")
//    return !~text.indexOf(val);
//}).hide();



    //$('th #T').each(function (col) {
    //    $(this).hover(
    //        function () {
    //            $(this).addClass('focus');
    //        },
    //        function () {
    //            $(this).removeClass('focus');
    //        }
    //    );

    //    $(this).click(function () {
    //        if ($(this).is('.asc')) {
    //            $(this).removeClass('asc');
    //            $(this).addClass('desc selected');
    //            sortOrder = -1;
    //        } else {
    //            $(this).addClass('asc selected');
    //            $(this).removeClass('desc');
    //            sortOrder = 1;
    //        }
    //        $(this).siblings().removeClass('asc selected');
    //        $(this).siblings().removeClass('desc selected');
    //        var arrData = $('#table').find('tbody >tr:has(td)').get();

    //        arrData.sort(function (a, b) {
    //            var val1 = $(a).children('td').eq(col).text().toUpperCase();
    //            var val2 = $(b).children('td').eq(col).text().toUpperCase();
    //            if ($.isNumeric(val1) && $.isNumeric(val2))
    //                return sortOrder === 1 ? val1 - val2 : val2 - val1;
    //            else
    //                return (val1 < val2) ? -sortOrder : (val1 > val2) ? sortOrder : 0;
    //        });
    //        $.each(arrData, function (index, row) {
    //            $('tbody').append(row);
    //        });
    //    });
    //});