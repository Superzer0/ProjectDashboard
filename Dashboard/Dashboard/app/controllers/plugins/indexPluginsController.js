(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('indexPluginsController', ['$scope', 'FileUploader', '$http', indexPluginsController]);

    function indexPluginsController($scope, FileUploader, $http) {

        var zipFileRegEx = /.+\.zip/i;

        $scope.pluginUpload = {
            fileId: "",
            validationResults: ""
        }

        $scope.uploader = new FileUploader({
            url: 'api/plugins/upload'
        });

        $scope.uploader.filters.push({
            name: 'fileCountFilter',
            fn: function (item, options) {
                console.log(item);
                return this.queue.length < 2;
            }
        });

        $scope.uploader.filters.push({
            name: 'fileFormatFilter',
            fn: function (item, options) {
                return zipFileRegEx.test(item.name);
            }
        });

        $scope.uploader.onSuccessItem = function (fileItem, response, status, headers) {
            console.info(response);
            $scope.pluginUpload.fileId = response.fileId;
        }

        $scope.uploader.onErrorItem = function (fileItem, response, status, headers) {
            console.error(response);
            $scope.pluginUpload.fileId = response;
        }

        $scope.validatePlugin = function () {
            $http.post('api/plugins/validate/' + $scope.pluginUpload.fileId)
                .success(function (response) {
                    $scope.pluginUpload.validationResults = "ok";
                })
                .error(function (err) {
                    $scope.pluginUpload.validationResults = err;
                });
        }

        activate();

        $scope.toogleMenu = function () {
            $("#wrapper").toggleClass("toggled");
        }

        function activate() { }
    }
})();
