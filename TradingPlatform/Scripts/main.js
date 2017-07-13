function changePasswordProgressBar(ev) {
    var wrost = 7,
        bad = /(?=.{8,}).*/,
        good = /^(?=\S*?[a-z])(?=\S*?[0-9])\S{8,}$/,
        better = /^(?=\S*?[A-Z])(?=\S*?[a-z])((?=\S*?[0-9])|(?=\S*?[^\w\*]))\S{8,}$/,
        best = /^(?=\S*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9])(?=\S*?[^\w\*])\S{8,}$/,
        password = $(ev.target).val(),
        strength = '0',
        progressClass = 'progress-bar progress-bar-',
        ariaMsg = '0% Complete (danger)',
        $progressBarElement = $('#password-progress-bar');
    if (best.test(password) === true) {
        strength = '100%';
        progressClass += 'success';
        ariaMsg = '100% Complete (success)';
    } else if (better.test(password) === true) {
        strength = '80%';
        progressClass += 'info';
        ariaMsg = '80% Complete (info)';
    } else if (good.test(password) === true) {
        strength = '50%';
        progressClass += 'warning';
        ariaMsg = '50% Complete (warning)';
    } else if (bad.test(password) === true) {
        strength = '30%';
        progressClass += 'warning';
        ariaMsg = '30% Complete (warning)';
    } else if (password.length >= 1 && password.length <= wrost) {
        strength = '10%';
        progressClass += 'danger';
        ariaMsg = '10% Complete (danger)';
    } else if (password.length < 1) {
        strength = '0';
        progressClass += 'danger';
        ariaMsg = '0% Complete (danger)';
    }
    $progressBarElement.removeClass().addClass(progressClass);
    $progressBarElement.attr('aria-valuenow', strength);
    $progressBarElement.css('width', strength);
    $progressBarElement.find('span.sr-only').text(ariaMsg);
}
$(document).ready(function () {
    $(".pwd").tooltip();
    $(".pwd").first().on('keyup', changePasswordProgressBar);
});
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip({
        'placement': 'auto right',
        'container': 'body'
    });
});
$(document).on("keypress", ":input:not(textarea):not([type=submit])", function (event) {
    if (event.keyCode == 13) {
        event.preventDefault();
        return false;
    }
});
$(document).ready(function () {
    if ($.validator && $.validator.unobtrusive) {
        $.validator.unobtrusive.adapters.addSingleVal("requiredlocalized", "size");
        $.validator.addMethod("require", function (value) {
            console.log("unobtrusive validate required attribute");
            return value != null;
        });
    }
});

function ApproveTrade(id) {
    $('#approveTradeId').val(id);
    $('#approveTradeIdText').text(id);
}
$(function () {
    $('body').on('click', '.details-toggle', function () {
        $(this).find('.show-details').toggle();
        $(this).find('.hide-details').toggle();
        $(this).next('.details-trading').toggle();
    });
});


function InitCountdown() {
    $('[data-countdown]:not(span.trade-update-span)').each(function () {
        var pattern = '%H:%M:%S';
        var dayspattern = '%-D:%H:%M:%S';
        var $this = $(this),
            finalDate = $(this).data('countdown');
        $this.countdown(finalDate, function (event) {
            if (((new Date(finalDate) - new Date()) / 1000 / 60 / 60 / 24) < 1) {
                $this.html(event.strftime(pattern));
            } else {
                $this.html(event.strftime(dayspattern));
            }
        }).on('finish.countdown', function (event) {
            setTimeout(500);
            var trade = $this.closest('table').attr("id");
            console.log(trade);
            window.location.href = "/trade/ShowCurrentTrades?trade=" + trade;
        });
    });
};

$(function () {
    $('[data-countdown]:not(span.trade-update-span)').each(function () {
        var pattern = '%H:%M:%S';
        var dayspattern = '%-D:%H:%M:%S';
        var $this = $(this),
            finalDate = $(this).data('countdown');
        $this.countdown(finalDate, function (event) {
            if (((new Date(finalDate) - new Date()) / 1000 / 60 / 60 / 24) < 1) {
                $this.html(event.strftime(pattern));
            } else {
                $this.html(event.strftime(dayspattern));
            }
        }).on('finish.countdown', function (event) {
            setTimeout(500);
            var trade = $this.closest('table').attr("id");
            console.log(trade);
            window.location.href = "/trade/ShowCurrentTrades?trade=" + trade;
        });
    });
});


$(function () {
    $('.trade-update-span').each(function () {
        var pattern = '%H:%M:%S';
        var dayspattern = '%-D:%H:%M:%S';
        var $this = $(this),
            finalDate = $(this).data('countdown');
        $this.countdown(finalDate, function (event) {
            if (((new Date(finalDate) - new Date()) / 1000 / 60 / 60 / 24) < 1) {
                $this.html(event.strftime(pattern));
            } else {
                $this.html(event.strftime(dayspattern));
            }
        }).on('finish.countdown', function (event) {
            var tradeId = $this.attr("data-tableid");
           // if ($(this))
            console.log('timeout 2 sec tradeId:' + tradeId + new Date());
            setTimeout(UpdateTrade(tradeId), 2000);
            console.log('updating after timeout...' + new Date());

        });

    });
});



function InitTourCountdown() {
    $('.trade-update-span').each(function () {
        var pattern = '%H:%M:%S';
        var dayspattern = '%-D:%H:%M:%S';
        var $this = $(this),
            finalDate = $(this).data('countdown');
        $this.countdown(finalDate, function (event) {
            if (((new Date(finalDate) - new Date()) / 1000 / 60 / 60 / 24) < 1) {
                $this.html(event.strftime(pattern));
            } else {
                $this.html(event.strftime(dayspattern));
            }
        }).on('finish.countdown', function (event) {
            var tradeId = $this.attr("data-tableid");
            
            console.log('timeout 2 sec tradeId:' + tradeId + new Date());
            setTimeout(UpdateTrade(tradeId), 2000);
            console.log('updating after timeout...' + new Date());
        });
    });
};
function UpdateTrade(auction) {
    
    $.ajax({
        url: "/Trade/RenderTradeView",
        type: "GET",
        data: {
            trade: auction,
            _: new Date()
        }
    }).done(function (partialViewResult) {
        console.log("Trade #" + auction + " table updated after bet");
        $('#tradeTable-' + auction).empty();
        $('#tradeTable-' + auction).html(partialViewResult);
        InitCountdown();
        InitTourCountdown();
        console.log("try click n details");
        $('.details-toggle[href="#details-' + auction + '"]').click();
    });
};




$(function () {
    $("input").each(function () {
        $(this).attr("autocomplete", "off");
    });
});

function showElement(id) {
    var el = document.getElementById(id);
    el.style.visibility = "visible";
    el.style.display = "block";
}

function hideElement(id) {
    var el = document.getElementById(id);
    el.style.visibility = "hidden";
    el.style.display = "none";
}
$(function () {
    if ($.connection != null) {
        $.connection.hub.logging = true;
        $.connection.hub.start();
        $.connection.hub.error(function (error) {
            console.log('SignalR error: ' + error);
        });
        var notifier = $.connection.notificationHub;
        $.connection.hub.disconnected(function () {
            setTimeout(function () {
                console.log("try reconnect");
                $.connection.hub.start();
            }, 1000);
        });
        $.connection.hub.start().done(function () {
            console.log("notifier started");
        });
        notifier.client.renderNotification = function (notification) {
            $.notify({
                title: notification.Subject,
                text: notification.Body,
                image: '<img src="/Content/images/envelop.png" alt="!" style="width:22px;height:16px;"/>',
                id: notification.Id
            }, {
                style: 'metro',
                className: 'white',
                autoHide: true,
                clickToHide: true,
                autoHideDelay: 6000,
                showAnimation: 'slideDown',
                showDuration: 400,
                hideAnimation: 'slideUp',
                hideDuration: 200
            });
        };
        notifier.client.showUsersCount = function (count) {
            console.log("try set users count to " + count);
            document.getElementById("usersCount").innerHTML = count;
        }
        notifier.client.goToTrade = function (trade) {
            window.location.href = "/trade/ShowCurrentTrades?trade=" + trade;
        }
        notifier.client.reloadTable = function (trade) {
            UpdateTrade(trade);
        }
        notifier.client.ShowTime = function (servertime) {
            console.log("try render time");
            var jsTime = new Date(servertime);
            if (jsTime == 'Invalid Date') {
                jsTime = new Date();
                console.log("error in handling datetime");
            }
            setInterval(function () {
                jsTime.setTime(jsTime.getTime() + (1000));
                $('#time').text(jsTime.toLocaleTimeString());
                $('#date em').text(jsTime.toLocaleTimeString('ru-RU', {
                    hour12: false,
                    hour: "numeric",
                    minute: "numeric"
                }).toString());
            }, 1000);
            setTimedifferenceCookie(jsTime.valueOf());
        };
        notifier.client.renderPartial = function (partialToString) {
            try {
                $('#currentTradeTable').empty();
                $('#currentTradeTable').html(partialToString);
            } catch (e) {
                alert(e);
            }
        };
        notifier.client.messageBet = function (data) {
            $("#chatroom ul").append("<li>" + data + "</li>");
        };
        notifier.client.reload = function () {
            window.location.reload();
        };
        $(document).on('click', '.notifyjs-metro-base', function () {
            notifier.server.setViewedNotification($(this).find('.id').html());
        });
    }
});
$(function getCountdown() {
    var timeBlock = $('#timeCountdown');
    if (!timeBlock.length) {
        return;
    }
    setInterval(function () {
        var date = new Date().toLocaleTimeString('ru-RU', {
            hour12: false,
            hour: "numeric",
            minute: "numeric",
            second: "numeric"
        }).toString(),
            dateArr = null,
            hours = null,
            minutes = null,
            seconds = null;
        dateArr = date.split(':');
        hours = dateArr[0];
        minutes = dateArr[1];
        seconds = dateArr[2];
        timeBlock.text(hours + ' : ' + minutes + ' : ' + seconds);
    }, 1000);
});
$(document).ready(function () {
    ['IsTaxPayer', 'isConfidant', 'OptionalBankBill'].forEach(function (element, index, array) {
        CheckBlockCCE(element, false);
    });
    CheckBlockCCE('PostAddress', true);
});

function CheckBlockCCE(id, revers) {
    if ((revers === false && $('#' + id).is(':checked')) || (revers === true && !$('#' + id).is(':checked'))) {
        ShowBlockCCE(id);
    } else {
        HideBlockCCE(id);
    }
};

function onSuccess(data) {
    if (data.Success === true) {
        $('#CancelModal').click();
    } else {
        $('#errorModal').text(data.error);
    }
};

function ShowBlockCCE(id) {
    $("div[name='" + id + "']").show();
    $("div[name='" + id + "'] input").each(function () {
        if (!$(this).hasClass('no-required')) {
            $(this).attr('required', 'required');
        }
    });
};

function HideBlockCCE(id) {
    $("div[name='" + id + "']").hide();
    $("div[name='" + id + "'] input").each(function () {
        $(this).removeAttr('required', 'required');
    });
};

function OpenModelTwo(url, btn) {
    $(btn).addClass("disabled");
    $('#modal_two_form').load(window.location.protocol + '//' + window.location.host + url + '&_=' + (new Date()).getTime(), function () {
        $('#modal_two_overlay').fadeIn(400, function () {
            $('#modal_two_form').css('display', 'block').animate({
                opacity: 1
                //top: '40%'
            }, 200);
            changeScale('left');
        });
        $('#modal_two_form .close, #modal_two_form .close-btn, #modal_two_overlay').click(function () {
            $(btn).removeClass("disabled");
            $('#modal_two_form').animate({
                opacity: 0
                //top: '0%'
            }, 200, function () {
                $(this).css('display', 'none');
                $('#modal_two_overlay').fadeOut(400);
                $('#modal_two_form').html('');
            });
            return false;
        });
    });
};

function changeScale(id) {
    if (id === 'left') {
        $('.left-dialog').css('transform', 'scale(1,1)').css('opacity', '1');//.css('left', '35px').css('z-index', '1041');
        $('.right-dialog').css('transform', 'scale(0.8,0.8)').css('opacity', '0.7');//.css('left', '-35px').css('z-index', '1040');
    } else {
        $('.right-dialog').css('transform', 'scale(1,1)').css('opacity', '1');//.css('left', '-35px').css('z-index', '1041');
        $('.left-dialog').css('transform', 'scale(0.8,0.8)').css('opacity', '0.7');//.css('left', '35px').css('z-index', '1040');
    }
}

function onTwoModelSuccess(data) {
    if (data.success === true) {
        $('#modal_two_form .close').click();
        $('.close').click();
    } else {
        $('#error').text(data.error);
    }
}

function onTwoModelFailure(data) { }

function OpenHelpDialog(page) {
    $('#help-modal-container .modal-body').load(window.location.protocol + '//' + window.location.host + '/home/help?page=' + page + '&_=' + (new Date()).getTime(), function () {
        $('#help-modal-container').modal('show');
    });
}
$(function () {
    $('body').on('click', '.modal-link1', function (e) {
        e.preventDefault();
        $(this).attr('data-target', '#modal-container');
        $(this).attr('data-toggle', 'modal');
    });
    $('body').on('click', '.modal-close-btn', function () {
        $('#modal-container').modal('hide');
    });
    $('#modal-container').on('hidden.bs.modal', function () {
        $(this).removeData('bs.modal');
    });
    $('#CancelModal').on('click', function () {
        return false;
    });
});
$(document).ready(function () {
    setTimezoneCookie();
   // setTimedifferenceCookie();
});

function setTimezoneCookie() {
    var timezone_cookie = "timezoneoffset";
    if (!$.cookie(timezone_cookie)) {
        $.cookie(timezone_cookie, new Date().getTimezoneOffset());
        location.reload();
    } else {
        var storedOffset = parseInt($.cookie(timezone_cookie));
        var currentOffset = new Date().getTimezoneOffset();
        if (storedOffset !== currentOffset) {
            $.cookie(timezone_cookie, new Date().getTimezoneOffset());
            location.reload();
        }
    }
};

function setTimedifferenceCookie(milliseconds) {
    var timediffCookie = "timediff";
    var diff = new Date() - new Date(milliseconds);
    $.cookie(timediffCookie, diff);
};



$(function () {
    var mob_height = $(window).innerHeight();
    if ($(window).innerWidth() < 768) {
        $(".aside-element hr").remove();
        $("nav .aside-element").addClass("nav navbar-nav");
        $(".aside-element ul").css("display", "none");
        $("nav .aside-element h3").append("<span class='caret'></span>")

        $("nav .aside-element h3").click(function () {
            if ($(this).hasClass("toggled")) {
                $(this).removeClass("toggled").siblings("ul").slideToggle("slow");
            } else {
                $("h3.toggled").removeClass("toggled").siblings("ul").slideToggle("slow");
                $(this).addClass("toggled").siblings("ul").slideToggle("slow");
            }
        });


        //$('button.navbar-toggle').attr('aria-expanded', 'true').on('click', function () {
        //    $('nav .aside-element h3.toggled').click();
        //});

        //$('button.navbar-toggle').on('click', function () {
        //    if ($(this).attr('aria-expanded') == 'true') {
        //        $('nav .aside-element h3.toggled').click();
        //    }
        //});

        //$('nav div.icon').on('click', function () {
        //    $('#mobile-nav-wrapper').slideToggle('slow');
        //    $('.nav-dropdown.toggled, .nav-second-dropdown.toggled').removeClass('toggled').children('ul').slideToggle();
        //});

        //$('.nav-dropdown').on('click', function () {
        //    $('.nav-second-dropdown.toggled').removeClass('toggled').children('ul').slideToggle('slow');
        //    $('.nav-second-dropdown').on('click', function (event) {
        //        event.stopPropagation();
        //    });
        //    if ($(this).hasClass('toggled')) {
        //        $(this).removeClass('toggled').children('ul').slideToggle('slow');
        //    } else {
        //        $('.nav-dropdown.toggled').removeClass('toggled').children('ul').slideToggle('slow');
        //        $(this).addClass('toggled').children('ul').slideToggle('slow');
        //    }
        //});

        //$('.nav-second-dropdown').on('click', function () {
        //    if ($(this).hasClass('toggled')) {
        //        $(this).removeClass('toggled').children('ul').slideToggle('slow');
        //    } else {
        //        $('.nav-second-dropdown.toggled').removeClass('toggled').children('ul').slideToggle('slow');
        //        $(this).addClass('toggled').children('ul').slideToggle('slow');
        //    }
        //});

        //footer accordeon
        $(".footer-item ul").css("display", "none");
        $(".footer-item h4").click(function () {
            if ($(this).hasClass("toggled")) {
                $(this).removeClass("toggled").siblings("ul").slideToggle("slow");
            } else {
                $(".footer-item h4.toggled").removeClass("toggled").siblings("ul").slideToggle("slow");
                $(this).addClass("toggled").siblings("ul").slideToggle("slow");
            }
        });
        //end footer accordeon

        //$("#mobile-top-bar .aside-element").wrapAll("<div id='mobile-aside-element-wrapper'></div>");
        //$("#mobile-aside-element-wrapper").css("max-width", "79.999%");


        //modile #lang panel insert
        //$("#lang-select").insertBefore("#navigation").css({
        //    "display": "block",
        //    "position": "absolute",
        //    "top": "70px",
        //    "left": "initial",
        //    "right": "4%",
        //    "max-width": "19.999%"
        //});
        
        //if ($("#mobile-aside-element-wrapper .aside-element").length > 1) {
        //    $("#mobile-top-bar #lang-select li").css({
        //        "display": "block",
        //        "margin" : "0 0 10px"
        //    }).parents('#lang-select').css('top', '77.5px');
        //} else {
        //    $("#mobile-top-bar #lang-select li").css({
        //        "display": "inline-block",
                
        //    });
        //}
        //end mobile #lang-select panel
    }
    //end width < 768

    //width >= 768
    if ($(window).innerWidth() >= 768) {

        $(window).scroll(function () {
            if ($(document).scrollTop() > 200) {
                $("#arrow-to-top").css("display", "block");
            } else {
                $("#arrow-to-top").css("display", "none");
            }
        });

        //$('.nav-second-dropdown').on({
        //    mouseenter: function () {
        //        $(this).parent().css('border-radius', '0');
        //        $('.nav-second-dropdown:last-child').css('border-radius', '0');
        //    },
        //    mouseleave: function () {
        //        $(this).parent().css('border-radius', '0 0 15px 15px');
        //        $('.nav-second-dropdown:last-child').css('border-radius', '0 0 15px 15px');
        //    }
        //});
        //$("#mobile-top-bar > .aside-element").detach();

        //$("ul").removeClass("nav navbar-nav navbar-right");

        //$("li.nav-dropdown").on({
        //    mouseenter: function () {
        //        $(this).css({
        //            "box-shadow": "none",
        //            //"border-radius": "15px 15px 0 0",
        //            "background-color": "#30736c"
        //        })
        //    },
        //    mouseleave: function () {
        //        if ($(this).hasClass("active")) {
        //            $(this).css({
        //                "box-shadow": "2px 4px 5px rgba(0,0,0,.5)",
        //                //"border-radius": "15px",
        //            })
        //        } else {
        //            $(this).css({
        //                "background-color": "initial"
        //            })
        //        }
        //    }
        //});

        //$('li.nav-second-dropdown').on({
        //    mouseenter: function () {
        //        $(this).parent().children('li:last-child').css('border-radius', '0 0 0 15px');
        //    },
        //    mouseleave: function () {
        //        $(this).parent().children('li:last-child').css('border-radius', '0 0 15px 15px');
        //    }
        //});

        //$(".aside-element h3").on({
        //    click: function () {
        //        if ($(this).hasClass("aside-element-clicked")) {
        //            $(this).removeClass("aside-element-clicked").siblings("ul").slideToggle();
        //        } else {
        //            $(this).css({ "color": "#333", "border-bottom": "1px solid #30736c"}).parent().css("background-color", "#eaeae4");
        //            $(".aside-element h3.aside-element-clicked").removeClass("aside-element-clicked").siblings("ul").slideToggle();
        //            $(this).addClass("aside-element-clicked").siblings("ul").slideToggle();
        //        }
        //    },
        //    mouseenter: function () {
        //        if (!$(this).hasClass("aside-element-clicked")) {
        //            $(this).css({ "color": "#f2ffff", "border-bottom": "1px solid white"}).parent().css("background-color", "#30736c");
        //        }
        //    },
        //    mouseleave: function () {
        //        $(this).css({ "color": "#333", "border-bottom": "1px solid #30736c"}).parent().css("background-color", "#eaeae4");
        //    }
        //})

    }

    //end width >= 768

    //width < 992

    if ($(window).innerWidth() < 992) {

        $('.icon').on('click', function () {
            if ($(this).hasClass('toggled')) {
                $('body').removeClass('noscroll');
                $(this).removeClass('toggled');
                $('.dropbtn.toggled').removeClass('toggled').next().slideToggle('slow');
                $('#top-wrapper').css('height', '50px')
                $('#nav-content').slideToggle('slow');
                //$('#top-wrapper').css({
                //    'height': '50px',
                //    'overflow': 'hidden',
                //    'bottom': 'initial'
                //});

            } else {
                $('body').addClass('noscroll');
                $(this).addClass('toggled');
                $('#top-wrapper').css('height', '100%');
                $('#nav-content').slideToggle('slow');
                //$('#top-wrapper').css({
                //    'top': '0',
                //    'bottom': '0',
                //    'left': '0',
                //    'right': '0',
                //    'height': '100%',
                //    'overflow-y': 'scroll'
                //});

            }
        });
        $('.dropbtn').on('click', function () {
            if ($(this).hasClass('toggled')) {
                $(this).removeClass('toggled').next().slideToggle('slow');
                $(this).children('span.caret').css('transform', 'rotate(-90deg)');
            } else {
                $('.dropbtn.toggled').removeClass('toggled').next().slideToggle('slow').siblings('p').children('span.caret').css('transform', 'rotate(-90deg)');
                $(this).addClass('toggled').next().slideToggle('slow');
                $(this).children('span.caret').css('transform', 'rotate(0deg)');
            }
        });
    }

    //end width < 992


    //width >= 992
    if ($(window).innerWidth() >= 992) {
        $(window).scroll(function () {
            if ($(document).scrollTop() > 200) {
                $('#top-wrapper').css('box-shadow', '0 2px 9px rgba(119, 119, 119, 1)');
            } else {
                $('#top-wrapper').css('box-shadow', 'none');
            }
        });
 
    }
    //end width >= 992

    //How to use ETP table collapse

    //$(".table-howituse-panel").append("<span class='howitusepanel-arrow'><img src='/Content/images/btn-rules.png'></span>")

    //$(".table-howituse-panel").click(function () {
    //    if ($(this).hasClass("howituse-panel-clicked")) {
    //        $(this).children("span").children("img").css({
    //            transform: "rotate(0deg)",
    //            transition: "transform 600ms"
                
    //        });
    //        $(this).removeClass("howituse-panel-clicked").next().css("display", "none");
    //    } else {
    //        $(".table-howituse-panel.howituse-panel-clicked").children("span").children("img").css({
    //            transform: "rotate(0deg)",
    //            transition: "transform 600ms"
    //        });
    //        $(".table-howituse-panel.howituse-panel-clicked").removeClass("howituse-panel-clicked").next().css("display", "none");
    //        $(this).children("span").children("img").css({
    //            transform: "rotate(90deg)",
    //            transition: "transform 600ms"
    //        });
    //        $(this).addClass("howituse-panel-clicked").next().css("display", "block");
    //    }

    //    //if ($(this).hasClass("howituse-panel-clicked")) {
    //    //    $(this).removeClass("howituse-panel-clicked").next().slideToggle(3000);
    //    //} else {
    //    //    $(".table-howituse-panel.howituse-panel-clicked").removeClass("howituse-panel-clicked").next().slideToggle(3000);
    //    //    $(this).addClass("howituse-panel-clicked").next().slideToggle(3000);
    //    //}
    //})

    //end Etp table collapse
});

$(".fa-battery-empty").mouseenter(function () {
    $(this).toggleClass("fa-battery-empty fa-battery-3");
});

$(".fa-battery-empty").mouseleave(function () {
    $(this).toggleClass("fa-battery-3 fa-battery-empty");
});