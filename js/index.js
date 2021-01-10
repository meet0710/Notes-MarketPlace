/*eslint-env es6*/
/* eslint-disable*/


/* ===========================
    Navigation
============================ */

/* SHow and Hide Navigation */

$(function () {

    //show/hide nav on page load

    showHideNav();

    $(window).scroll(function () {

        //show/hide nav on window's scroll
        showHideNav();

    });

    function showHideNav() {

        if ($(window).scrollTop() > 50) {

			
            //Show White Nav
            $("nav").addClass("white-nav-top");

            //Show Dark logo
            $(".navbar-brand img").attr("src", "home/logo.png");

            

        } else {

            //Hide White Nav
            $("nav").removeClass("white-nav-top");

            //Show Logo
            $(".navbar-brand img").attr("src", "img/pre-login/top-logo.png");

        }

    }

});

/* ===========================
        Mobile Menu
============================ */

$(function() {

    //Show mobile navigation
    $("#mobile-nav-open-btn").click(function() {
        
        $("#mobile-nav").css("height", "100%");
        
    });
    
    //Hide mobile navigation
    $("#mobile-nav-close-btn, #mobile-nav a").click(function() {
        
        $("#mobile-nav").css("height", "0%");
        
    });
    
});
