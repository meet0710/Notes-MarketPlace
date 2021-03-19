/*eslint-env es6*/
/* eslint-disable*/

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




/* ===========================
      Pagination
============================ */

$(function() {
    if($('.pagination-control').length) {
        $('.pagination-control').each(function() {
            
            //paginator element
            var pageControl = $(this);

            //element to be paginated
            var paginate = $(pageControl.attr('pagination-data'));

            //no of element shown per page
            var elementsShown = parseInt(paginate.attr('data-tr-per-page'),10);

            //no of total elements
            var totalElements = paginate.children();

            //no of total pages
            var numPages = Math.ceil(totalElements.length/elementsShown); 

            //Max page shown in paginator (incomplete function  to show limited pages link in paginator)
            var maxPage = parseInt(pageControl.attr('max-page-shown'));

            totalElements.hide();

            // add links to paginator for all pages
            for(i=numPages;i>0;i--){

                //max page (incomplte function)
                if(i>maxPage){
                    pageControl.find('nav ul>li:first').after('<li style="display:none;" class="page-item"><a class="page-no page-link" page="'+i+'">'+i+'</a></li>');
                } else {

                    pageControl.find('nav ul>li:first').after('<li class="page-item"><a class="page-no page-link" page="'+i+'">'+i+'</a></li>');
                }
            }

            //click function to show page and hide current page
            pageControl.find('.page-no').on('click',function() {

                //active class change 
                pageControl.find('nav ul>li a').removeClass('active');
              $(this).addClass('active');

                var page = parseInt($(this).attr('page'));
                totalElements.hide(); 

                var showStart = (page-1)*elementsShown;

                //calculate and show next max element per page
                totalElements.slice(showStart,((showStart+elementsShown)<totalElements.length) ? (showStart+elementsShown) : totalElements.length).fadeIn('slow');
            });


            //trigger click on first page to show initial page
            pageControl.find('.page-no:first').trigger('click');

            //nprevious page trigger click on previos page link
            pageControl.find('nav ul>li:first').on('click',function() {
                pageControl.find('nav ul>li a.active').parent().prev().find('a.page-no').trigger('click');

            });

            //next page trigger click on next page link
            pageControl.find('nav ul>li:last').on('click',function() {

                pageControl.find('nav ul>li a.active').parent().next().find('a.page-no').trigger('click');

            });
        });
    }
});

$(function(){
	$(".dropdown").click(function(){
		$(this).find(".dropdown-content").toggleClass("show");
	})
})

$(document).on("click", function(event){
        var $trigger = $(".dropdown");
        if($trigger !== event.target && !$trigger.has(event.target).length){
            $(".dropdown-content").removeClass("show");
        }            
    });


/*  =====================================
    Search in Table
    ===================================== */

$(function() {
        $('.search-in-table').each(function(){

        //table to be searched
        var tableBody = $($(this).attr('data-table-id')+' tbody');    
        var tableBodyString = $($(this).attr('data-table-id')+' tbody'); 

        $(this).on('click', function() {

            var resultsFound = false;
            //search text
            var text = $($(this).attr('data-search-input')).val();
            if(!text.trim()){
                return
            }

            text = text.toLowerCase();
            var doesItContainText;

            //search tbody
            tableBody.find('tr').filter(function(){

                doesItContainText = $(this).text().toLowerCase().indexOf(text) > -1;
                $(this).toggle(doesItContainText);
                if(doesItContainText) {
                    resultsFound = true;
                }

            });

            if(!resultsFound) {
                var colSpan = tableBody.find('tr:first').children().length;
                tableBody.append('<tr class="no-results"><td class="text-center" colspan="'+colSpan+'">No Record Found</td></tr>');
                setTimeout(function() {

                    tableBody.find('.no-results').remove();

                    //trigger click on first page to show initial page
                    $('.pagination-control[pagination-data="'+tableBodyString+'"]').find('.page-no:first').trigger('click');

                },3000);
            }

        });

    });
});