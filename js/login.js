$(function (){
    
    $('.show-pass').on('click', function() {
        var currentState = $(this).find('img').attr('toggle');
        
        if($(currentState).attr('type') == "password"){
            $(currentState).attr('type','text');
        } else{
            $(currentState).attr('type','password');
        }
        
    });
    
});