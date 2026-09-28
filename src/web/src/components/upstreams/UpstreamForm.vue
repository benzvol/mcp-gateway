<script setup lang="ts">
import { useRouter } from 'vue-router'

import { Alert, AlertDescription } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Switch } from '@/components/ui/switch'
import TransportFields from './TransportFields.vue'
import AuthFields from './AuthFields.vue'
import ConnectionTestPanel from './ConnectionTestPanel.vue'
import { useUpstreamForm } from '@/composables/useUpstreamForm'
import { useFieldErrors } from '@/composables/useFieldErrors'
import { useCreateUpstream, useUpdateUpstream } from '@/queries/upstreams'
import type { UpstreamResponse } from '@/types/upstream'

const props = defineProps<{
  initial?: UpstreamResponse
}>()

const router = useRouter()
const form = useUpstreamForm(props.initial)
const fieldErrors = useFieldErrors()
const createUpstream = useCreateUpstream()
const updateUpstream = useUpdateUpstream()

const saving = form.isEdit ? updateUpstream.isPending : createUpstream.isPending

function submit() {
  fieldErrors.clear()

  if (form.isEdit && props.initial) {
    updateUpstream.mutate(
      { id: props.initial.id, body: form.toUpdateRequest() },
      {
        onSuccess: () => router.push({ name: 'upstreams' }),
        onError: (err) => fieldErrors.apply(err),
      },
    )
  } else {
    createUpstream.mutate(form.toCreateRequest(), {
      onSuccess: () => router.push({ name: 'upstreams' }),
      onError: (err) => fieldErrors.apply(err),
    })
  }
}

function cancel() {
  router.push({ name: 'upstreams' })
}
</script>

<template>
  <form @submit.prevent="submit">
    <div class="mb-6 flex items-start justify-between gap-6">
      <h1 class="text-2xl font-bold">{{ form.isEdit ? 'Edit upstream' : 'Add upstream' }}</h1>
      <div class="flex gap-2">
        <Button type="button" variant="secondary" @click="cancel">Cancel</Button>
        <Button type="submit" :disabled="saving">Save upstream</Button>
      </div>
    </div>

    <Alert v-if="fieldErrors.conflictMessage.value" variant="destructive" class="mb-4">
      <AlertDescription>{{ fieldErrors.conflictMessage.value }}</AlertDescription>
    </Alert>

    <Card>
      <CardContent class="pt-6">
        <div class="mb-5 flex flex-col gap-1.5">
          <Label>Name <span class="text-primary">*</span></Label>
          <Input v-model="form.state.name" :aria-invalid="!!fieldErrors.errorFor('name')" />
          <p v-if="fieldErrors.errorFor('name')" class="text-xs text-destructive">
            {{ fieldErrors.errorFor('name') }}
          </p>
        </div>

        <div class="mb-5 flex flex-row items-center gap-2.5">
          <Switch v-model="form.state.enabled" />
          <span class="text-xs text-muted-foreground">
            A disabled upstream isn't proxied and its tools stay hidden.
          </span>
        </div>

        <TransportFields
          v-model:transport="form.state.transport"
          v-model:command="form.state.command"
          v-model:endpoint="form.state.endpoint"
          :args="form.args"
          :environment="form.environment"
          :headers="form.headers"
          :command-error="fieldErrors.errorFor('command')"
          :endpoint-error="fieldErrors.errorFor('endpoint')"
        />

        <h2
          class="my-6 border-b border-border pb-2 text-xs tracking-wide text-muted-foreground uppercase"
        >
          Authentication
        </h2>
        <AuthFields
          v-model:auth-kind="form.state.authKind"
          :headers="form.headers"
          :secret-input="form.secret.input"
          :has-stored-secret="form.hasStoredSecret.value"
          :auth-config-input="form.authConfigJson.input"
          :has-stored-auth-config="form.hasStoredAuthConfig.value"
          @secret-input="form.markSecretDirty"
          @secret-clear="form.clearSecret"
          @auth-config-input="form.markAuthConfigDirty"
          @auth-config-clear="form.clearAuthConfig"
        />

        <h2
          class="my-6 border-b border-border pb-2 text-xs tracking-wide text-muted-foreground uppercase"
        >
          Connection test
        </h2>
        <ConnectionTestPanel :build-request="form.toCreateRequest" />
      </CardContent>
    </Card>
  </form>
</template>
