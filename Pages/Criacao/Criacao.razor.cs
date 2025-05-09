﻿using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using modulum.Application.Models;
using modulum.Application.Requests.Dynamic.Create;
using modulum.Application.Requests.Dynamic.Update;
using modulum.Application.Validators.Requests.Dynamic;
using modulum.Client.Infrastructure.FormValidators;
using modulum.Client.Infrastructure.Services;
using modulum.Shared;
using modulum.Shared.Enum;
using modulum.Shared.Wrapper;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Modulum.Client.Pages.Criacao
{
    public partial class Criacao
    {
        private FluentValidationValidator _fluentValidationValidator;
        private FormValidator _criacaoFormValidator = new();
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        //private List<CreateDynamicFieldRequest> _modelCampos = new();
        private DynamicForm _modelDynamic = new DynamicForm();

        private bool _loading = false;
        private bool IsView { get; set; } = false;
        private bool _loadingDados { get; set; }

        [Parameter] public int TableId { get; set; }
        [Parameter] public string Operation { get; set; }

        private async Task AdicionarCampo()
        {
            _loadingService.Show();
            _criacaoFormValidator.ClearAllErrors();
            var validationResult = await new CreateDynamicFieldRequestValidator().ValidateAsync(_modelDynamic.fieldRequest);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    _criacaoFormValidator.DisplayAllErrors(new Dictionary<string, string> { { error.PropertyName, error.ErrorMessage } }, _modelDynamic.fieldRequest);
                }
                _loadingService.Hide();
                return;
            }


            if (!string.IsNullOrWhiteSpace(_modelDynamic.fieldRequest.NomeCampoTela))
            {
                var index = _modelDynamic.tableRequest.Campos.FindIndex(campo => campo.Id == _modelDynamic.fieldRequest.Id &&
                            (campo.Id != null || campo.TempId == _modelDynamic.fieldRequest.TempId));
                if (index != -1)
                {
                    _modelDynamic.tableRequest.Campos[index] = _modelDynamic.fieldRequest;
                }
                else
                {
                    _modelDynamic.tableRequest.Campos.Add(new CreateDynamicFieldRequest
                    {
                        NomeCampoTela = _modelDynamic.fieldRequest.NomeCampoTela,
                        NomeCampoBase = _modelDynamic.fieldRequest.NomeCampoTela.Replace(" ", "_"),
                        Tipo = _modelDynamic.fieldRequest.Tipo,
                        Tamanho = _modelDynamic.fieldRequest.Tamanho,
                        IsPrimaryKey = false,
                        IsObrigatorio = _modelDynamic.fieldRequest.IsObrigatorio,
                        TempId = Guid.NewGuid()
                    });
                }
                _modelDynamic.fieldRequest = new CreateDynamicFieldRequest(); // limpa
            }
            _loadingService.Hide();
        }

        protected override async Task OnInitializedAsync()
        {
            await CarregarDados();
        }

        protected override async Task OnParametersSetAsync()
        {
            await CarregarDados();
        }

        private async Task CarregarDados()
        {
            _loadingDados = true;
            _loadingService.Show();
            _modelDynamic = new DynamicForm();
            IsView = false;
            if (Operation.Equals("create"))
            {
                _modelDynamic.tableRequest.CampoPK = "Id";
                _modelDynamic.tableRequest.Campos.Add(new CreateDynamicFieldRequest
                {
                    NomeCampoTela = "Id",
                    NomeCampoBase = "Id",
                    IsObrigatorio = true,
                    IsPrimaryKey = true,
                    Tamanho = null,
                    Tipo = (TypeColumnEnum?)TypeColumnEnum.INT,
                    TempId = Guid.NewGuid()
                });
            }
            else
            {
                var response = await _dynamicManager.GetMapTable(TableId);
                if (response != null)
                {
                    if (response.Succeeded)
                    {
                        _modelDynamic.tableRequest = response.Data;
                    }
                }
                if (Operation.Equals("select"))
                {
                    IsView = true;
                }
            }
            _loadingDados = false;
            _loadingService.Hide();
            await Task.CompletedTask;
        }

        private void ClearTela() 
        {
        
        }

        private bool _tamanhoDisabled = true;

        private void AddApiErrors(IResult response)
        {
            _criacaoFormValidator.ClearAllErrors();
            _criacaoFormValidator.DisplayAllErrors(response.Fields);
        }

        private void RemoverCampo(CreateDynamicFieldRequest campo)
        {
            _loadingService.Show();
            _modelDynamic.tableRequest.Campos.Remove(campo);
            _loadingService.Hide();
        }

        private void EditarCampo(CreateDynamicFieldRequest campo)
        {
            _loadingService.Show();
            if (campo.TempId == null)
            {
                campo.TempId = Guid.NewGuid();
            }
            _modelDynamic.fieldRequest = campo;
            _loadingService.Hide();
        }

        public async Task DoSalvarAsync()
        {
            _loadingService.Show();
            _loading = true;
            if (!Validated)
            {
                _loading = false;
                _loadingService.Hide();
                return;
            }
            _criacaoFormValidator.ClearAllErrors();
            _modelDynamic.tableRequest.NomeTabela = _modelDynamic.tableRequest.NomeTela?.Replace(" ", "_");
            
            var validationResult = await new CreateDynamicTableRequestValidator().ValidateAsync(_modelDynamic.tableRequest);
            
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    _criacaoFormValidator.DisplayAllErrors(new Dictionary<string, string> { { error.PropertyName, error.ErrorMessage } }, _modelDynamic.tableRequest);
                }
                _loading = false;
                _loadingService.Hide();
                return;
            }
            var response = await _dynamicManager.OperacaoMapTable(_modelDynamic.tableRequest, Operation);
            if (response.Messages == null || response.Messages.Count == 0)
            {
                _snackBar.Add("Tela cadastrada com sucesso", Severity.Success);
            }
            else
            {
                _snackBar.Add(response.Messages.FirstOrDefault(), response.Succeeded ? Severity.Success : Severity.Error);
            }
            if (response != null)
            {
                if (response.Succeeded)
                {
                    _menuService.NotifyMenuChanged();
                    _loading = false;
                    _loadingService.Hide();
                    _navigationManager.NavigateTo("/");
                }
                else
                {
                    // Tratar erro aqui
                    _loading = false;
                    _loadingService.Hide();
                    return;
                }
            }
            else
            {
                _loading = false;
                _loadingService.Hide();
            }
        }

        public string GetDisplayText(Enum value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Type type = value.GetType();
            if (Attribute.IsDefined(type, typeof(FlagsAttribute)))
            {
                var sb = new System.Text.StringBuilder();

                foreach (Enum field in Enum.GetValues(type))
                {
                    if (Convert.ToInt64(field) == 0 && Convert.ToInt32(value) > 0)
                        continue;

                    if (value.HasFlag(field))
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");

                        var f = type.GetField(field.ToString());
                        var da = (DisplayAttribute)Attribute.GetCustomAttribute(f, typeof(DisplayAttribute));
                        sb.Append(da?.ShortName ?? da?.Name ?? field.ToString());
                    }
                }

                return sb.ToString();
            }
            else
            {
                var f = type.GetField(value.ToString());
                if (f != null)
                {
                    var da = (DisplayAttribute)Attribute.GetCustomAttribute(f, typeof(DisplayAttribute));
                    if (da != null)
                        return da.ShortName ?? da.Name;
                }
            }
            return value.ToString();
        }

        private void OnTipoChanged()
        {
            if (_modelDynamic.fieldRequest.Tipo != TypeColumnEnum.VARCHAR)
            {
                _modelDynamic.fieldRequest.Tamanho = null;
            }
            _criacaoFormValidator.ClearAllErrors();
        }
    }
}
