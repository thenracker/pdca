/**
 * This sorting plug-in for DataTables will correctly sort data in date time or date
 * format typically used in Germany:
 * date and time:`dd.mm.YYYY HH:mm`
 * just date:`dd.mm.YYYY`.
 *
 * Please note that this plug-in is **deprecated*. The
 * [datetime](//datatables.net/blog/2014-12-18) plug-in provides enhanced
 * functionality and flexibility.
 *
 *  @name Date (dd.mm.YYYY) or date and time (dd.mm.YYYY HH:mm)
 *  @summary Sort date / time in the format `dd.mm.YYYY HH:mm` or `dd.mm.YYYY`.
 *  @author [Ronny Vedrilla](http://www.ambient-innovation.com)
 *  @deprecated
 *
 *  @example
 *    $('#example').dataTable( {
 *       columnDefs: [
 *         { type: 'de_datetime', targets: 0 },
 *         { type: 'de_date', targets: 1 }
 *       ]
 *    } );
 */

jQuery.extend(jQuery.fn.dataTableExt.oSort, {
    "de_datetime-asc": function (a, b) {
        var x, y;
        if (jQuery.trim(a) !== '') {
            var deDatea = jQuery.trim(a).split(' ');
            var deTimea = deDatea[1].split(':');
            var deDatea2 = deDatea[0].split('.');

            //Moje úprava
            for (i = 0; i < 3; i++) {
                if (deDatea2[i].length == 1)
                    deDatea2[i] = "0" + deDatea2[i];
            }
            for (i = 0; i < 2; i++) {
                if (deTimea[i].length == 1)
                    deTimea[i] = "0" + deTimea[i];
            }
            

            x = (deDatea2[2] + deDatea2[1] + deDatea2[0] + deTimea[0] + deTimea[1]) * 1;
        } else {
            x = Infinity; // = l'an 1000 ...
        }

        if (jQuery.trim(b) !== '') {
            var deDateb = jQuery.trim(b).split(' ');
            var deTimeb = deDateb[1].split(':');
            deDateb = deDateb[0].split('.');

            //Moje úprava
            for (i = 0; i < 3; i++) {
                if (deDateb[i].length == 1)
                    deDateb[i] = "0" + deDateb[i];
            }
            for (i = 0; i < 2; i++) {
                if (deTimeb[i].length == 1)
                    deTimeb[i] = "0" + deTimeb[i];
            }

            y = (deDateb[2] + deDateb[1] + deDateb[0] + deTimeb[0] + deTimeb[1]) * 1;
        } else {
            y = Infinity;
        }
        var z = ((x < y) ? -1 : ((x > y) ? 1 : 0));
        return z;
    },

    "de_datetime-desc": function (a, b) {
        var x, y;
        if (jQuery.trim(a) !== '') {
            var deDatea = jQuery.trim(a).split(' ');
            var deTimea = deDatea[1].split(':');
            var deDatea2 = deDatea[0].split('.');

            //Moje úprava
            for (i = 0; i < 3; i++) {
                if (deDatea2[i].length == 1)
                    deDatea2[i] = "0" + deDatea2[i];
            }
            for (i = 0; i < 2; i++) {
                if (deTimea[i].length == 1)
                    deTimea[i] = "0" + deTimea[i];
            }

            x = (deDatea2[2] + deDatea2[1] + deDatea2[0] + deTimea[0] + deTimea[1]) * 1;
        } else {
            x = Infinity;
        }

        if (jQuery.trim(b) !== '') {
            var deDateb = jQuery.trim(b).split(' ');
            var deTimeb = deDateb[1].split(':');
            deDateb = deDateb[0].split('.');

            //Moje úprava
            for (i = 0; i < 3; i++) {
                if (deDateb[i].length == 1)
                    deDateb[i] = "0" + deDateb[i];
            }
            for (i = 0; i < 2; i++) {
                if (deTimeb[i].length == 1)
                    deTimeb[i] = "0" + deTimeb[i];
            }

            y = (deDateb[2] + deDateb[1] + deDateb[0] + deTimeb[0] + deTimeb[1]) * 1;
        } else {
            y = Infinity;
        }
        var z = ((x < y) ? 1 : ((x > y) ? -1 : 0));
        return z;
    },

    "de_date-asc": function (a, b) {
        var x, y;
        if (jQuery.trim(a) !== '') {
            var deDatea = jQuery.trim(a).split('.');
            
            //Moje úprava
            for (i = 0; i < 3; i++) {
                if (deDatea[i].length == 1)
                    deDatea[i] = "0" + deDatea[i];
            }


            x = (deDatea[2] + deDatea[1] + deDatea[0]) * 1;
        } else {
            x = Infinity; // = l'an 1000 ...
        }

        if (jQuery.trim(b) !== '') {
            var deDateb = jQuery.trim(b).split('.');

            //Moje úprava
            for (i = 0; i < 3; i++) {
                if (deDateb[i].length == 1)
                    deDateb[i] = "0" + deDateb[i];
            }

            y = (deDateb[2] + deDateb[1] + deDateb[0]) * 1;
        } else {
            y = Infinity;
        }
        var z = ((x < y) ? -1 : ((x > y) ? 1 : 0));
        return z;
    },

    "de_date-desc": function (a, b) {
        var x, y;
        if (jQuery.trim(a) !== '') {
            var deDatea = jQuery.trim(a).split('.');

            //Moje úprava
            for (i = 0; i < 3; i++) {
                if (deDatea[i].length == 1)
                    deDatea[i] = "0" + deDatea[i];
            }

            x = (deDatea[2] + deDatea[1] + deDatea[0]) * 1;
        } else {
            x = Infinity;
        }

        if (jQuery.trim(b) !== '') {
            var deDateb = jQuery.trim(b).split('.');

            //Moje úprava
            for (i = 0; i < 3; i++) {
                if (deDateb[i].length == 1)
                    deDateb[i] = "0" + deDateb[i];
            }

            y = (deDateb[2] + deDateb[1] + deDateb[0]) * 1;
        } else {
            y = Infinity;
        }
        var z = ((x < y) ? 1 : ((x > y) ? -1 : 0));
        return z;
    }
});
