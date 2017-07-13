
ko.bindingHandlers.datetimepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var options = allBindingsAccessor().dateTimePickerOptions || {
            autoclose: true,
            todayBtn: true,
            'format': 'dd.mm.yyyy hh:ii',
            language: 'uk',
            startDate: new Date(),
            minuteStep: 30,
            initialDate: new Date()
        };
        $(element).datetimepicker(options).on("changeDate", function (ev) {
            var observable = valueAccessor();
            observable(ev.date);
        });
    },
    update: function (element, valueAccessor) {
        ko.utils.unwrapObservable(valueAccessor());
    }
};

ko.bindingHandlers.datePicker = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var unwrap = ko.utils.unwrapObservable;
        var dataSource = valueAccessor();
        var binding = allBindingsAccessor();
        var options = {
            keyboardNavigation: true,
            todayHighlight: true,
            autoclose: true,
            //daysOfWeekDisabled: [0, 6],
            format: 'dd.mm.yyyy',
            language: 'uk'
        };
        if (binding.datePickerOptions) {
            options = $.extend(options, binding.datePickerOptions);
        }
        $(element).datepicker(options);
        $(element).datepicker('update', dataSource());
        $(element).on("changeDate", function (ev) {
            var observable = valueAccessor();
            if ($(element).is(':focus')) {
                // Don't update while the user is in the field...
                // Instead, handle focus loss
                $(element).one('blur', function (ev) {
                    var dateVal = $(element).datepicker("getDate");
                    observable(dateVal);
                });
            }
            else {
                observable(ev.date);
            }
        });
        //handle removing an element from the dom
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).datepicker('remove');
        });
    },
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        //   $(element).datepicker('update', value);
    }
};


ko.bindingHandlers.option = {
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        ko.selectExtensions.writeValue(element, value);
    }
};
    

viewModel.Nomenclatures = ko.computed(function () {
    var product = viewModel.SelectedProduct();
    if (product === null || product === undefined) return null;
    return ko.utils.arrayFilter(product.Dependencies(), function (dep) {
        return dep.Type() === 'Nomenclature';
    });
}, viewModel);

viewModel.Gosts = ko.computed(function () {
    var product = viewModel.SelectedProduct();
    if (product === null || product === undefined) return null;
    return ko.utils.arrayFilter(product.Dependencies(), function (dep) {
        return dep.Type() === 'GOST';
    });
}, viewModel);
viewModel.Incotherms = ko.computed(function () {
    var product = viewModel.TransportTypeSelected();
    if (product === null || product === undefined) return null;
    return ko.utils.arrayFilter(product.Dependencies(), function (dep) {
        return dep.Type() === 'Incotherms';
    });
}, viewModel);
viewModel.Places = ko.computed(function () {
    var product = viewModel.TransportTypeSelected();
    if (product === null || product === undefined) return null;
    return ko.utils.arrayFilter(product.Dependencies(), function (dep) {
        return dep.Type() === 'ShipmentPoint';
    });
}, viewModel);

viewModel.RailwayBegins = ko.computed(function () {
    var product = viewModel.RailwayTransportTherms()[0].Dependencies()[0];
    if (product === null || product === undefined) return null;
    return ko.utils.arrayFilter(product.Dependencies(), function (dep) {
        return dep.Type() === 'TransporTherm';
    });
}, viewModel);

viewModel.RailwayEnds = ko.computed(function () {
    var product = viewModel.RailwayTransportTherms()[0].Dependencies()[1];
    if (product === null || product === undefined) return null;
    return ko.utils.arrayFilter(product.Dependencies(), function (dep) {
        return dep.Type() === 'TransporTherm';
    });
}, viewModel);

viewModel.executeOnServer = function (model, url) {
    $.ajax({
        url: url,
        type: 'POST',
        data: ko.mapping.toJSON(model),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        beforeSend: function () {
            $(".fa-spin-hover").removeClass("nodisplay");
            $('#submitBtn').innerHtml = "";
            $('#submitBtn').attr('disabled', 'disabled');

            //$("#submitBtn").disabled = true;
            //$("#submitBtn").innerHTML = '<i class="fa fa-spinner fa-3x fa-spin-hover"></i> …';
        },
        success: function (data) {
            if (data.Success === true) {
                window.location.href = data.redirectUrl;
            } else {
                alert("Error: " + data.responseText);
            }
        },
        error: function (error) { alert("Виникла помилка, спробуйте пізніше" + error.toString());}
    });
};

var $loading = $('#load').hide();
$(document)
  .ajaxStart(function () {
      $loading.show();
  })
  .ajaxStop(function () {
      $loading.hide();
  });