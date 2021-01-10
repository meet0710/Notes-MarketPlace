/*eslint-env es6*/
/* eslint-disable*/

/* ===========================
        Modal
============================ */
/*
var modal = document.getElementById("myModal");

// Get the button that opens the modal
var btn = document.getElementById("download");

// Get the <span> element that closes the modal
var span = document.getElementsByClassName("close")[0];

// When the user clicks the button, open the modal 
btn.onclick = function() {
  modal.style.display = "block";
}

// When the user clicks on <span> (x), close the modal
span.onclick = function() {
  modal.style.display = "none";
}

// When the user clicks anywhere outside of the modal, close it
window.onclick = function(event) {
  if (event.target == modal) {
    modal.style.display = "none";
  }
}
*/

var modal = document.getElementById("myModal");
function openmodal() {
	
	modal.style.display = "block";
	
	window.onclick = function(event) {
  if (event.target == modal) {
    modal.style.display = "none";
  }
}
}

function closemodal() {
	var modal = document.getElementById("myModal");
	 modal.style.display = "none";
}



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