$(document).ready(function () {
    //$(".datepicker").datepicker({
    //format: 'mm/dd/yyyy',
    //changeMonth: true,
    //changeYear: true,
    ////startDate: '0d',
    //onSelect: function (dateText) {
    //    alert("gg");
    //    //var prevDate = $(this).data("prev")
    //    //var curDate = dateText;
    //    //if(prevDate == curDate){
    //    //  $(this).data("prev", curDate)
    //    //  calcUpdate();
    //    //    }
    //    }
    //});

    
    $(".mask-integer").inputmask("integer", { autoGroup: true, groupSeparator: ",", groupSize: 3 });
    $(".mask-decimal").inputmask("decimal", { autoGroup: true, groupSeparator: ",", groupSize: 3, digits: 2 });
    $(".box .box-header .actions a.box-collapse").attr('title', 'expand/collapse');
    $(".box .box-header .title").click(function () {
        wireBoxTitleExpandCollapse(this);
    });

    $(".box .box-header .title").css("cursor", "pointer");
    $(".box .box-header .title").attr("title", "expand/collapse");
});

function wireBoxTitleExpandCollapse(sender) {
    
    var open = $(sender).parent().parent().hasClass('box-collapsed');

    if (open) {
        $(sender).parent().parent().removeClass("box-collapsed");
    } else {
        $(sender).parent().parent().addClass("box-collapsed");
    }
}

function addNotification(title, message,type) {
    $.growl({
        title: title,
        message: message,
        type:type,
    });
}

function showBlockModal(title,message) {
    $('#block-modal-title').html(title);
    $('#block-modal-body').html(message);
    $('#block-modal').modal();

    return true;
}

function openWindow(url,height,width) {
    window.open(url, "_blank", "scrollbars=1,resizable=1,height="+height+",width="+width+"");
}
