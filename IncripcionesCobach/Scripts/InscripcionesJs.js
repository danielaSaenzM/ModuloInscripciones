
$(function () {
    var dataTable = $("#dtItems").DataTable({
        "paging": false,
        "ordering": false,
        "info": false,
        "searching": false
    });
    dataTable.rows().every(function (rowIdx, tableLoop, rowLoop) {
        var data = this.data();

        $('input[name ="concepto"]').val(data[1]);
        $('input[name ="importe"]').val(data[2].replace(',', ''));
        //$('input[name ="importe"]').val("1.00");
        $('input[name ="referencia"]').val($("#Referencia").val());
        //$('input[name ="referencia"]').val("99110000");
        $("#txtPago").val(data[2].replace(',', ''));
    });
});

function genPDFPagoAll() {
    html2canvas(document.getElementById('FichasPago')).then(
        function (canvas) {
            var doc = new jsPDF("p", "mm", "a4");
            var width = doc.internal.pageSize.getWidth();
            var height = doc.internal.pageSize.getHeight();
            var img = canvas.toDataURL('image/png');

            doc.addImage(img, 'JPEG', 5, 5, 200, 240);
            doc.save('Fichas_de_Pago.pdf');
        }

    );
}
function genPDFPago() {
    html2canvas(document.getElementById('inscripcion')).then(
        function (canvas) {
            var doc = new jsPDF("p", "mm", "a4");
            var width = doc.internal.pageSize.getWidth();
            var height = doc.internal.pageSize.getHeight();
            var img = canvas.toDataURL('image/png');

            doc.addImage(img, 'JPEG', 5, 5, 200, 80);
            doc.save('inscripcion.pdf');
        }

    );
}
function genPDFPagoSA() {
    html2canvas(document.getElementById('sociedad')).then(
        function (canvas) {
            var doc = new jsPDF("p", "mm", "a4");
            var width = doc.internal.pageSize.getWidth();
            var height = doc.internal.pageSize.getHeight();
            var img = canvas.toDataURL('image/png');

            doc.addImage(img, 'JPEG', 5, 5, 200, 80);
            doc.save('sociedadAlumnos.pdf');
        }

    );
}
function genPDFPagoPrevee() {
    html2canvas(document.getElementById('prevee')).then(
        function (canvas) {
            var doc = new jsPDF("p", "mm", "a4");
            var width = doc.internal.pageSize.getWidth();
            var height = doc.internal.pageSize.getHeight();
            var img = canvas.toDataURL('image/png');

            doc.addImage(img, 'JPEG', 5, 5, 200, 80);
            doc.save('PREVEE.pdf');
        }

    );
}