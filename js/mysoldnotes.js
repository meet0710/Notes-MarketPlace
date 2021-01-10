/*eslint-env es6*/
/* eslint-disable*/
/*eslint-env browser*/

/* ===========================
        Dropdown
============================ */
/* When the user clicks on the button,
toggle between hiding and showing the dropdown content */
function myFunction() {
  document.getElementById("myDropdown").classList.toggle("show");
}

// Close the dropdown menu if the user clicks outside of it
window.onclick = function(event) {
  if (!event.target.matches('.dropbtn')) {
    var dropdowns = document.getElementsByClassName("dropdown-content");
    var i;
    for (i = 0; i < dropdowns.length; i++) {
      var openDropdown = dropdowns[i];
      if (openDropdown.classList.contains('show')) {
        openDropdown.classList.remove('show');
      }
    }
  }
}



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



$(document).ready(function () {
	$('#tableData').paging({
		limit: 6
	});
});
