(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('pluginUploadWizardController', ['$scope', 'FileUploader', '$http', pluginUploadWizardController]);

    function pluginUploadWizardController($scope, FileUploader, $http) {

        var zipFileRegEx = /.+\.zip/i;

        $scope.pluginUpload = {
            fileId: "",
            validationResults: "",
            pluginInfo: "",
            installationResult: undefined
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
                    $scope.pluginUpload.validationResults = response;
                })
                .error(function (err) {
                    $scope.pluginUpload.validationResults = err;
                });
        }

        $scope.getInformation = function () {
            $http.post('api/plugins/info/' + $scope.pluginUpload.fileId)
               .success(function (response) {
                   $scope.pluginUpload.pluginInfo = response;
               })
               .error(function (err) {
                   $scope.pluginUpload.pluginInfo = err;
               });
        }

        $scope.installPlugin = function () {
            $http.post('api/plugins/install/' + $scope.pluginUpload.fileId)
               .success(function (response) {
                   $scope.pluginUpload.installationResult = "Ok";
               })
               .error(function (err) {
                   $scope.pluginUpload.installationResult = err;
               });
        }

        $scope.startOver = function () {
            $scope.pluginUpload = {
                fileId: "",
                validationResults: "",
                pluginInfo: "",
                installationResult: undefined
            }
        }
    }
})();
