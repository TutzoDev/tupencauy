(function($) {
  "use strict"; // Start of use strict

  // Toggle the side navigation
  $("#sidebarToggle, #sidebarToggleTop").on('click', function(e) {
    $("body").toggleClass("sidebar-toggled");
    $(".sidebar").toggleClass("toggled");
    if ($(".sidebar").hasClass("toggled")) {
      $('.sidebar .collapse').collapse('hide');
    };
  });

  // Close any open menu accordions when window is resized below 768px
  $(window).resize(function() {
    if ($(window).width() < 768) {
      $('.sidebar .collapse').collapse('hide');
    };
    
    // Toggle the side navigation when window is resized below 480px
    if ($(window).width() < 480 && !$(".sidebar").hasClass("toggled")) {
      $("body").addClass("sidebar-toggled");
      $(".sidebar").addClass("toggled");
      $('.sidebar .collapse').collapse('hide');
    };
  });

  // Prevent the content wrapper from scrolling when the fixed side navigation hovered over
  $('body.fixed-nav .sidebar').on('mousewheel DOMMouseScroll wheel', function(e) {
    if ($(window).width() > 768) {
      var e0 = e.originalEvent,
        delta = e0.wheelDelta || -e0.detail;
      this.scrollTop += (delta < 0 ? 1 : -1) * 30;
      e.preventDefault();
    }
  });

  // Scroll to top button appear
  $(document).on('scroll', function() {
    var scrollDistance = $(this).scrollTop();
    if (scrollDistance > 100) {
      $('.scroll-to-top').fadeIn();
    } else {
      $('.scroll-to-top').fadeOut();
    }
  });

  // Smooth scrolling using jQuery easing
  $(document).on('click', 'a.scroll-to-top', function(e) {
    var $anchor = $(this);
    $('html, body').stop().animate({
      scrollTop: ($($anchor.attr('href')).offset().top)
    }, 1000, 'easeInOutExpo');
    e.preventDefault();
  });

  // BEGIN: Dashboard-specific code

  // Fetch the last match result from the API
  // $.ajax({
  //   url: 'https://api.example.com/last-match-result',
  //   method: 'GET',
  //   success: function(data) {
  //     const tableBody = $('#lastMatchResultTable tbody');
  //     const match = data.match;
  //     const row = `
  //       <tr>
  //         <td>${match.date}</td>
  //         <td>${match.homeTeam}</td>
  //         <td>${match.awayTeam}</td>
  //         <td>${match.result}</td>
  //       </tr>
  //     `;
  //     tableBody.append(row);
  //   },
  //   error: function(error) {
  //     console.error('Error fetching last match result:', error);
  //   }
  // });

  // // Fetch the next match info from the API
  // $.ajax({
  //   url: 'https://api.example.com/next-match',
  //   method: 'GET',
  //   success: function(data) {
  //     const nextMatchInfo = $('#nextMatchInfo');
  //     if (data.userHasBet) {
  //       const match = data.match;
  //       const content = `
  //         <p><strong>Fecha:</strong> ${match.date}</p>
  //         <p><strong>Equipo Local:</strong> ${match.homeTeam}</p>
  //         <p><strong>Equipo Visitante:</strong> ${match.awayTeam}</p>
  //       `;
  //       nextMatchInfo.append(content);
  //     } else {
  //       const warningMessage = `
  //         <div class="alert alert-danger" role="alert">
  //           No has realizado una apuesta para este partido.
  //         </div>
  //       `;
  //       nextMatchInfo.append(warningMessage);
  //     }
  //   },
  //   error: function(error) {
  //     console.error('Error fetching next match info:', error);
  //   }
  // });

  // END: Dashboard-specific code

})(jQuery); // End of use strict
