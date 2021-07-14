/***************Custom Class for handling ui in test results *****************/
function TestResultUI() {
}

TestResultUI.showMoreCookieName = "TEST_RESULT_SHOW_MORE";
TestResultUI.expandCollapseCookieName = "TEST_RESULT_EXP_COLLAPSE";

TestResultUI.expandCollapseAllResults =
function (expand) {
    if (expand) {
        $('.box-result').removeClass("box-collapsed");
        $.cookie(TestResultUI.expandCollapseCookieName, 'true');
    } else {
        $('.box-result').addClass("box-collapsed");
        $.cookie(TestResultUI.expandCollapseCookieName, 'false');
    }
};

TestResultUI.showHideMore =
function (sender) {
    var showMore = ($(sender).attr('zoom-state') == "out");

    if (showMore) {
        $('.advance-display').show();
        $(sender).attr('zoom-state', 'in');
        $(sender).find('i').removeClass('icon-zoom-in').addClass('icon-zoom-out');
        $(sender).find('span').html('Compact View');
        $(sender).attr('title','Click to see compact result view');
        $.cookie(TestResultUI.showMoreCookieName, 'true');
    } else {
        $('.advance-display').hide();
        $(sender).attr('zoom-state', 'out');
        $(sender).find('i').removeClass('icon-zoom-out').addClass('icon-zoom-in');
        $(sender).find('span').html('Detailed View');
        $(sender).attr('title', 'Click to see detailed result view with all columns');
        $.cookie(TestResultUI.showMoreCookieName, 'false');
    }
};


$(document).ready(function () {
    var expandTestView = $.cookie(TestResultUI.expandCollapseCookieName);
    if (typeof expandTestView != "undefined" && expandTestView == 'true') {
        TestResultUI.expandCollapseAllResults(true);
    } 

    

    var showMore = $.cookie(TestResultUI.showMoreCookieName);
    if (typeof showMore != "undefined" && showMore == 'true') {
        $('#showHideMoreButton').attr('zoom-state', 'out');
        TestResultUI.showHideMore($('#showHideMoreButton'));
    } else {
        $('.advance-display').hide();
        $('#showHideMoreButton').attr('title', 'Click to see detailed result view with all columns');
    }

});