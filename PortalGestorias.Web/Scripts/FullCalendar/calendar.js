// Custom jQuery
// ----------------------------------- 


(function (window, document, $, undefined) {





    if (!$.fn.fullCalendar) return;

    // When dom ready, init calendar and events
    $(function () {

        // The element that will display the calendar
        var calendar = $('#calendar');

        initExternalEvents(calendar);

        initCalendar(calendar);

    });


    // global shared var to know what we are dragging
    var draggingEvent = null;

    /**
     * ExternalEvent object
     * @param jQuery Object elements Set of element as jQuery objects
     */
    var ExternalEvent = function (elements) {

        if (!elements) return;

        elements.each(function () {
            var $this = $(this);
            // create an Event Object (http://arshaw.com/fullcalendar/docs/event_data/Event_Object/)
            // it doesn't need to have a start or end
            var calendarEventObject = {
                title: $.trim($this.text()) // use the element's text as the event title
            };

            // store the Event Object in the DOM element so we can get to it later
            $this.data('calendarEventObject', calendarEventObject);

            // make the event draggable using jQuery UI
            $this.draggable({
                zIndex: 1070,
                revert: true, // will cause the event to go back to its
                revertDuration: 0  //  original position after the drag
            });

        });
    };

    /**
     * Invoke full calendar plugin and attach behavior
     * @param  jQuery [calElement] The calendar dom element wrapped into jQuery
     * @param  EventObject [events] An object with the event list to load when the calendar displays
     */
    function getMesPorIdParteHoras(IdParteHoras) {
        var mesCalendario = 0;
    
        $.ajax({
            url: JSPath + '/api/Asignaciones/GetNumMes/',
            data: { 'IdParteHoras': IdParteHoras },
            async: false,
            success: function (result) {
                mesCalendario = result;
            }
        });
        return mesCalendario;
    }
    function getAñoPorIdParteHoras(IdParteHoras) {
        var añoCalendario = 0;
        $.ajax({
            url: JSPath +  '/api/Asignaciones/GetNumAño/',
            data: { 'IdParteHoras': IdParteHoras },
            async: false,
            success: function (result) {
                añoCalendario = result;
            }
        });
        return añoCalendario;
    }
    function initCalendar(calElement) {        

        // check to remove elements from the list
        var removeAfterDrop = $('#remove-after-drop');

        var IdParteHoras = getGET();
        var mesCalendario = getMesPorIdParteHoras(IdParteHoras);
        if (mesCalendario.toString().length == 1) { mesCalendario = "0" + mesCalendario; }
        var añoCalendario = getAñoPorIdParteHoras(IdParteHoras);

        var FechaMostrar = añoCalendario.toString() + "-" + mesCalendario.toString() + "-01"; 
        calElement.fullCalendar({
            locale: 'es',

            defaultDate: FechaMostrar,//"2014-09-04",

            showNonCurrentDates: false,
            header:
            {
                left: 'prev,next',
                center: 'title'
            },
            buttonIcons:
          { // note the space at the beginning
              prev: ' fa fa-caret-left',
              next: ' fa fa-caret-right'
          },
            buttonText:
          {
              today: 'Hoy'
          },
            editable: true,
            droppable: true, // this allows things to be dropped onto the calendar 
            drop: function (date, allDay) { // this function is called when something is dropped

                var $this = $(this),
                      // retrieve the dropped element's stored Event Object
                      originalEventObject = $this.data('calendarEventObject');

                // if something went wrong, abort
                if (!originalEventObject) return;

                // clone the object to avoid multiple events with reference to the same object
                var clonedEventObject = $.extend({}, originalEventObject);

                // assign the reported date
                clonedEventObject.start = date.time('08:30:00');
                clonedEventObject.allDay = true;
                clonedEventObject.backgroundColor = $this.css('background-color');
                clonedEventObject.borderColor = $this.css('border-color');
                clonedEventObject.editable = true;
                clonedEventObject.id = --newId;
                

                // render the event on the calendar
                // the last `true` argument determines if the event "sticks" 
                // (http://arshaw.com/fullcalendar/docs/event_rendering/renderEvent/)
                calElement.fullCalendar('renderEvent', clonedEventObject);

                // if necessary remove the element from the list
                if (removeAfterDrop.is(':checked')) {
                    $this.remove();
                }
            },
            eventDragStart: function (event, js, ui) {
                draggingEvent = event;
            },            

            // This array is the events sources
            events: JSPath +  '/api/Asignaciones/Imputaciones/' + getGET(),
            dayRender: function (date, cell) {
                if (date.isoWeekday() > 5) 
                    cell.addClass("fc-nonbusiness");                    
                for (var i = 0; i < festivos.length; i++) {
                    if (festivos[i].diff(date) == 0)
                        cell.addClass("fc-holiday");
                }    
                var f = new Date();
                var fechaCalendario = new Date(date);
                
                var hora = 0;
                hora = getHorasTotalDias(fechaCalendario);
                cell.append('<div class="unavailable"><input type="text" style="width:75px;border:none" value="Total: ' + hora + '"/></div>');                                
            },
            eventMouseover: function (calEvent, jsEvent) {
                var tooltip = '<div class="tooltipevent" style="width:auto;height:auto;background:#ccc;position:absolute;z-index:10001;">' + calEvent.title + '</div>';
                var $tooltip = $(tooltip).appendTo('body');

                $(this).mouseover(function (e) {
                    $(this).css('z-index', 10000);
                    $tooltip.fadeIn('500');
                    $tooltip.fadeTo('10', 1.9);
                }).mousemove(function (e) {
                    $tooltip.css('top', e.pageY + 10);
                    $tooltip.css('left', e.pageX + 20);
                });
            },
            eventMouseout: function (calEvent, jsEvent) {
                $(this).css('z-index', 8);
                $('.tooltipevent').remove();
            },                       
            eventClick: function (calEvent, jsEvent, view) {
                $('#eventInfo').first().data('event', calEvent);
                if ((calEvent.start.hours() + calEvent.start.minutes() / 60) == 0)
                {
                    var FechaInicio = new Date(calEvent.start);
                    var FechaFin = new Date(calEvent.end);
                    var FInicio = FechaInicio.getDate();
                    var FFin = FechaFin.getDate();
                    var EsViernes = false;
                    if (calEvent.end == null) {
                        var dia = 1;
                        var recomienzo = moment(calEvent.start);
                        if (recomienzo.isoWeekday() == 5) {
                            EsViernes = true;
                        }
                    }
                    var horasImputar = getHorasImputar(EsViernes);
                    $('#horas').val(horasImputar);
                }
                else
                {
                    $('#horas').val(calEvent.start.hours() + calEvent.start.minutes() / 60);
                }
                var IdParteHoras = getGET();
                var Estado = getEstadoParteHoras(IdParteHoras);
                var GestorParteHoras = getGestorParteHoras();
                if ((Estado == "ENVIADO" || Estado == "APROBADO") && GestorParteHoras == "ko") {
                    $('#eventInfo').modal('hide');
                }
                else if (calEvent.idFase != 0) {
                    $('#NombreProyecto').val(calEvent.title);                    
                    $('#eventInfo').modal('show');                    
                }
                
                return false;
            },            
            eventAfterAllRender: function (view) {
                var dayEvents = $('#calendar').fullCalendar('clientEvents', function (event) {
                    //if (event.end) {
                    //    var dates = getDates(event.start, event.end);
                    //    $.each(dates, function (index, value) {
                    //        var td = $('td.fc-day[data-date="' + value + '"]');
                    //        td.find('div:first').remove();
                    //    });
                    //} else {
                    //    var td = $('td.fc-day[data-date="' + event.start.format('YYYY-MM-DD') + '"]');
                    //    td.find('div:first').remove();
                    //}
                });
            }
        });

        function getHorasImputar(EsViernes) {
            var IdParteHoras = getGET();
        
            var horas = 0;
            $.ajax({
                url: JSPath +  '/api/Asignaciones/GetHorasImputarPorDefecto/?&EsViernes=' + EsViernes,
                data: { 'IdParteHoras': IdParteHoras },
                async: false,
                success: function (result) {
                    horas = result;
                },
                error: function (error) {
                    alert(error);
                }
            });
            return horas;

        }

        function getDates(startDate, endDate) {
            var now = startDate,
                dates = [];

            while (now.format('YYYY-MM-DD') <= endDate.format('YYYY-MM-DD')) {
                dates.push(now.format('YYYY-MM-DD'));
                now.add('days', 1);
            }
            return dates;
        };

    }

    function getHorasTotalDias(Fecha) {
        var IdParteHoras = getGET();
        var horas = 0;
        $.ajax({
            url: JSPath + '/api/Asignaciones/TotalHorasDias/?&IdParteHoras=' + IdParteHoras,
            data: { 'Fecha': Fecha },
            async: false,
            success: function (result) {
                horas = result;
            },
            error: function (error) {
                alert(error);
            }
        });
        return horas;
    }

    function getGET() {
        var loc = document.location.href;
        var getString = loc.split('/')[loc.split('/').length - 1];
        //var GET = getString.split('&');
        //var get = {};//this object will be filled with the key-value pairs and returned.

        //for (var i = 0, l = GET.length; i < l; i++) {
        //    var tmp = GET[i].split('=');
        //    get[tmp[0]] = unescape(decodeURI(tmp[1]));
        //}
        return getString;
    }

    function getEstadoParteHoras(IdParteHoras) {
        var Estado = null;
      
        $.ajax({
            url: JSPath +  '/api/Asignaciones/EstadoParteHoras',
            data: { 'IdParteHoras': IdParteHoras },
            async: false,
            success: function (result) {
                Estado = result;
            },
            error: function (error) {
                alert(error);
            }
        });

        return Estado;
    }


    function getGestorParteHoras() {
        var SiesGestor = "";
       
        $.ajax({
            url: JSPath +  '/api/Asignaciones/ComprobarSiesGestor',
            //data: { 'IdParteHoras': IdParteHoras },
            async: false,
            success: function (result) {
                SiesGestor = result;
            },
            error: function (error) {
                alert(error);
            }
        });
        return SiesGestor;
    }
    /**
     * Inits the external events panel
     * @param  jQuery [calElement] The calendar dom element wrapped into jQuery
     */
    function initExternalEvents(calElement) {
        // Panel with the external events list
        var externalEvents = $('.external-events');

        // init the external events in the panel
        new ExternalEvent(externalEvents.children('div'));

        // External event color is danger-red by default
        var currColor = '#f6504d';
        // Color selector button
        var eventAddBtn = $('.external-event-add-btn');
        // New external event name input
        var eventNameInput = $('.external-event-name');
        // Color switchers
        var eventColorSelector = $('.external-event-color-selector .circle');

        // Trash events Droparea 
        $('.external-events-trash').droppable({
            accept: '.fc-event',
            activeClass: 'active',
            hoverClass: 'hovered',
            tolerance: 'touch',
            drop: function (event, ui) {

                // You can use this function to send an ajax request
                // to remove the event from the repository

                if (draggingEvent) {
                    var eid = draggingEvent.id || draggingEvent._id;
                    // Remove the event
                    calElement.fullCalendar('removeEvents', eid);
                    // Remove the dom element
                    ui.draggable.remove();
                    // clear
                    draggingEvent = null;
                }
            }
        });

        eventColorSelector.click(function (e) {
            e.preventDefault();
            var $this = $(this);

            // Save color
            currColor = $this.css('background-color');
            // De-select all and select the current one
            eventColorSelector.removeClass('selected');
            $this.addClass('selected');
        });

        eventAddBtn.click(function (e) {
            e.preventDefault();

            // Get event name from input
            var val = eventNameInput.val();
            // Dont allow empty values
            if ($.trim(val) === '') return;

            // Create new event element
            var newEvent = $('<div/>').css({
                'background-color': currColor,
                'border-color': currColor,
                'color': '#fff'
            })
                .html(val);

            // Prepends to the external events list
            externalEvents.prepend(newEvent);
            // Initialize the new event element
            new ExternalEvent(newEvent);
            // Clear input
            eventNameInput.val('');
        });
    }


})(window, document, window.jQuery);


